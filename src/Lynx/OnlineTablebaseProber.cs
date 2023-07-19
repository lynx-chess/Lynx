using Lynx.Model;
using Microsoft.Extensions.Http;
using NLog;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lynx;

/// <summary>
/// https://syzygy-tables.info/ -
/// https://tablebase.lichess.ovh/
/// </summary>
public static class OnlineTablebaseProber
{
    public const int NoResult = 6666;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly static AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, 10 + retryAttempt)));    // 128, 256, 512, 1024ms

    private readonly static HttpClient _client = new(
        new PolicyHttpMessageHandler(_retryPolicy)
        {
            InnerHandler = new SocketsHttpHandler() { PooledConnectionLifetime = TimeSpan.FromMinutes(15) }
        })
    {
        BaseAddress = new("http://tablebase.lichess.ovh/")
    };

    private readonly static JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumMemberConverter() },
        TypeInfoResolver = SourceGenerationContext.Default
    };

    public static (int MateScore, Move BestMove) RootSearch(Position position, Dictionary<long, int> positionHistory, int halfMovesWithoutCaptureOrPawnMove, CancellationToken cancellationToken)
    {
        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || position.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return (NoResult, default);
        }

        var fen = position.FEN(halfMovesWithoutCaptureOrPawnMove);
        _logger.Debug("[{0}] Querying online tb for position {1}", nameof(RootSearch), fen);

        var tablebaseEval = GetEvaluation(fen, cancellationToken).Result;

        if (tablebaseEval is null || tablebaseEval.Category == TablebaseEvaluationCategory.Unknown)
        {
            _logger.Warn("An answer was expected from online tablebase for position {0}", fen);
            return (NoResult, default);
        }

        TablebaseEvalMove? bestMove = null;
        int mate = 0;

        IEnumerable<int>? allPossibleMoves = null;

        switch (tablebaseEval.Category)
        {
            case TablebaseEvaluationCategory.Unknown:
                return (NoResult, default);

            // No moves in mainline endpoint if there's a draw
            case TablebaseEvaluationCategory.Draw:
                if (tablebaseEval.IsStalemate)
                {
                    return (NoResult, default);
                }

                mate = 0;
                bestMove = tablebaseEval.Moves?.Find(m => m.Category == TablebaseEvaluationCategory.Draw);

                break;

            case TablebaseEvaluationCategory.MaybeWin:
            case TablebaseEvaluationCategory.Win:
                if (tablebaseEval.DistanceToMate.HasValue)
                {
                    mate = +(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                }
                else
                {
                    mate = +49;
                }

                if (Math.Abs(tablebaseEval.DistanceToZero ?? 0) + halfMovesWithoutCaptureOrPawnMove > 100)
                {
                    _logger.Info("Cursed win due to already {0} moves without captures/pawn moves {1}", halfMovesWithoutCaptureOrPawnMove, fen);
                    // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                }

                // for 3K4/2P5/1k6/8/8/8/8/8_w_-_-_1_1
                // http://tablebase.lichess.ovh/standard?fen=3K4/2P5/1k6/8/8/8/8/8_w_-_-_1_1
                // {"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":false,"dtz":1,"precise_dtz":1,"dtm":13,"category":"win", "moves":[
                //      { "uci":"c7c8q","san":"c8=Q","zeroing":true,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":false,"dtz":-12,"precise_dtz":-12,"dtm":-12,"category":"loss//     "},
                //      { "uci":"d8d7","san":"Kd7","zeroing":false,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":false,"dtz":-2,"precise_dtz":-2,"dtm":-16,"category":"loss//        "},
                //      { "uci":"d8c8","san":"Kc8","zeroing":false,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":false,"dtz":-4,"precise_dtz":-4,"dtm":-20,"category":"loss//        "},
                //      { "uci":"c7c8r","san":"c8=R","zeroing":true,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":false,"dtz":-22,"precise_dtz":-22,"dtm":-22,"category":"loss//     "},
                //      { "uci":"c7c8b","san":"c8=B","zeroing":true,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":true,"dtz":0,"precise_dtz":0,"dtm":0,"category":"draw"},
                //      { "uci":"c7c8n","san":"c8=N+","zeroing":true,"checkmate":false,"stalemate":false,"variant_win":false,"variant_loss":false,"insufficient_material":true,"dtz":0,"precise_dtz":0,"dtm":0,"category":"draw"},
                // If we follow dtz, we reach http://tablebase.lichess.ovh/standard?fen=3K4/2P5/1k6/8/8/8/8/8_w_-_-_1_1#2, which suggests to repeat
                // (rep 1) 29. Kd8 Kc5 30. Kd7 Kb6 (rep 2) 31.Kd6 Kb7 32.Kd7 (unica) Kb6 (rep 3)

                var bestMoveList = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Loss)
                    .OrderByDescending(m => m.DistanceToMate ?? 0)      // When winning, moves have negative dtm and dtz
                    .ThenByDescending(m => m.DistanceToZero ?? 0);

                if (bestMoveList is not null)
                {
                    allPossibleMoves ??= position.AllPossibleMoves();
                    var originalNumberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);

                    foreach (var move in bestMoveList)
                    {
                        if (!MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate))
                        {
                            throw new AssertException($"{move!.Uci} should be parsable from position {fen}");
                        }

                        var newPosition = new Position(in position, moveCandidate.Value);

                        var oldValue = halfMovesWithoutCaptureOrPawnMove;
                        halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveCandidate.Value, halfMovesWithoutCaptureOrPawnMove);
                        var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                        // If a repetition already exists, we can't rely on two fold repetition detection,
                        // so we check if it adds any extra repetitions and initially discard it if that's the case
                        var numberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);
                        if (numberOfTwoFoldRepetitionCount == originalNumberOfTwoFoldRepetitionCount && !Position.Is50MovesRepetition(halfMovesWithoutCaptureOrPawnMove))        // Attacking: any move that draws is discarded
                        {
                            bestMove = move;
                            break;
                        }

                        halfMovesWithoutCaptureOrPawnMove = oldValue;
                        Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                    }

                    if (bestMove is null)
                    {
                        // Since there are no moves that don't cause an extra 'two-fold' repetition, let's grab one that doesn't cause a three-fold one, hoping that the rival can't complete it
                        foreach (var move in bestMoveList)
                        {
                            MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate);
                            var newPosition = new Position(in position, moveCandidate!.Value);
                            var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                            if (!Position.IsThreefoldRepetition(positionHistory))
                            {
                                Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                                bestMove = move;
                                break;
                            }
                            Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                        }
                        if (bestMove is null)
                        {
                            _logger.Info("Can't find a path to win in position {fen} due to repetitions via all the possible candidate moves :O", fen);
                            mate = 0;
                            bestMove = bestMoveList.FirstOrDefault();
                        }
                    }
                }

                break;

            case TablebaseEvaluationCategory.Loss:
            case TablebaseEvaluationCategory.MaybeLoss:
                if (tablebaseEval.DistanceToMate.HasValue)
                {
                    mate = -(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                }
                else
                {
                    mate = -49;
                }

                if (Math.Abs(tablebaseEval.DistanceToZero ?? 0) + halfMovesWithoutCaptureOrPawnMove > 100)
                {
                    _logger.Info("Blessed loss due to already {0} moves without captures/pawn moves {1}", halfMovesWithoutCaptureOrPawnMove, fen);
                    // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                }

                // When defending, we priorize the highest dtz over dtm
                bestMoveList = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Win)
                    .OrderByDescending(m => m.DistanceToZero ?? 0)    // When losing, moves have positive dtm and dtz, so we want the highest
                    .ThenByDescending(m => m.DistanceToMate ?? 0);

                if (bestMoveList is not null)
                {
                    allPossibleMoves ??= position.AllPossibleMoves();

                    foreach (var move in bestMoveList)
                    {
                        if (!MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate))
                        {
                            throw new AssertException($"{move!.Uci} should be parsable from position {fen}");
                        }

                        var newPosition = new Position(in position, moveCandidate.Value);

                        var oldValue = halfMovesWithoutCaptureOrPawnMove;
                        halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveCandidate.Value, halfMovesWithoutCaptureOrPawnMove);
                        var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                        if (Position.IsThreefoldRepetition(positionHistory) || Position.Is50MovesRepetition(halfMovesWithoutCaptureOrPawnMove))     // Defending: any possible move that draws is good
                        {
                            bestMove = move;
                            break;
                        }

                        halfMovesWithoutCaptureOrPawnMove = oldValue;
                        Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                    }

                    if (bestMove is not null)
                    {
                        _logger.Info("There's a miraculous move ({0}) that saves {fen} due to repetition :O", bestMove.Uci, fen);
                        mate = 0;
                    }
                    else
                    {
                        // Since there are no moves that cause a 'three-fold' repetition, let's grab one that causes an extra two-fold one, hoping that the rival completes it or allows us to do so next move
                        var originalNumberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);

                        foreach (var move in bestMoveList)
                        {
                            MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate);
                            var newPosition = new Position(in position, moveCandidate!.Value);
                            var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                            // If a repetition already exists, we can't rely on two fold repetition detection,
                            // so we check if it adds any extra repetitions
                            var numberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);
                            if (numberOfTwoFoldRepetitionCount > originalNumberOfTwoFoldRepetitionCount)
                            {
                                Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                                bestMove = move;
                                break;
                            }
                            Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                        }

                        bestMove ??= bestMoveList.FirstOrDefault();
                    }
                }

                break;

            case TablebaseEvaluationCategory.CursedWin:
                // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                if (tablebaseEval.DistanceToMate.HasValue)
                {
                    mate = +(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                }
                else if (tablebaseEval.DistanceToZero.HasValue)
                {
                    mate = +(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToZero.Value));
                }
                else
                {
                    mate = +51;
                }

                _logger.Info("Cursed win {0}", fen);

                bestMoveList = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.BlessedLoss)
                    .OrderByDescending(m => m.DistanceToMate ?? 0)      // When winning, moves have negative dtm and dtz
                    .ThenByDescending(m => m.DistanceToZero ?? 0);

                if (bestMoveList is not null)
                {
                    allPossibleMoves ??= position.AllPossibleMoves();
                    var originalNumberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);

                    foreach (var move in bestMoveList)
                    {
                        if (!MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate))
                        {
                            throw new AssertException($"{move!.Uci} should be parsable from position {fen}");
                        }

                        var newPosition = new Position(in position, moveCandidate.Value);

                        var oldValue = halfMovesWithoutCaptureOrPawnMove;
                        halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveCandidate.Value, halfMovesWithoutCaptureOrPawnMove);
                        var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                        // If a repetition already exists, we can't rely on two fold repetition detection,
                        // so we check if it adds any extra repetitions and initially discard it if that's the case
                        var numberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);
                        if (numberOfTwoFoldRepetitionCount == originalNumberOfTwoFoldRepetitionCount && !Position.Is50MovesRepetition(halfMovesWithoutCaptureOrPawnMove))    // Attacking: any move that draws is discarded
                        {
                            bestMove = move;
                            break;
                        }

                        halfMovesWithoutCaptureOrPawnMove = oldValue;
                        Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                    }

                    if (bestMove is null)
                    {
                        // Since there are no moves that don't cause an extra 'two-fold' repetition, let's grab one that doesn't cause a three-fold one, hoping that the rival can't complete it
                        foreach (var move in bestMoveList)
                        {
                            MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate);
                            var newPosition = new Position(in position, moveCandidate!.Value);
                            var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                            if (!Position.IsThreefoldRepetition(positionHistory))
                            {
                                Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                                bestMove = move;
                                break;
                            }
                            Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                        }
                        if (bestMove is null)
                        {
                            _logger.Info("All moves draw earlier than the expected cursed win due to repetitions :O");
                            mate = 0;
                            bestMove = bestMoveList.FirstOrDefault();
                        }
                    }
                }

                break;

            case TablebaseEvaluationCategory.BlessedLoss:
                // We don't set mate to 0 since we don't really care about it due to being root node search, and we have to play the best moves anyway
                if (tablebaseEval.DistanceToMate.HasValue)
                {
                    mate = -(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                }
                else if (tablebaseEval.DistanceToZero.HasValue)
                {
                    mate = -(int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToZero.Value));
                }
                else
                {
                    mate = -51;
                }

                _logger.Info("Blessed loss {0}", fen);

                bestMoveList = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.CursedWin)
                    .OrderByDescending(m => m.DistanceToZero ?? 0)      // When winning, moves have positive dtm and dtz, so we want the highest
                    .ThenByDescending(m => m.DistanceToMate ?? 0);

                if (bestMoveList is not null)
                {
                    allPossibleMoves ??= position.AllPossibleMoves();
                    foreach (var move in bestMoveList)
                    {
                        if (!MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate))
                        {
                            throw new AssertException($"{move!.Uci} should be parsable from position {fen}");
                        }

                        var newPosition = new Position(in position, moveCandidate.Value);

                        var oldValue = halfMovesWithoutCaptureOrPawnMove;
                        halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveCandidate.Value, halfMovesWithoutCaptureOrPawnMove);
                        var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                        if (Position.IsThreefoldRepetition(positionHistory) || Position.Is50MovesRepetition(halfMovesWithoutCaptureOrPawnMove)) // Defending: any possible move that draws is good
                        {
                            bestMove = move;
                            break;
                        }

                        halfMovesWithoutCaptureOrPawnMove = oldValue;
                        Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                    }

                    if (bestMove is not null)
                    {
                        _logger.Info("Move {0} draws the game due to repetition earlier than the expected blessed loss in {fen} position :O", bestMove.Uci, fen);
                        mate = 0;
                    }
                    else
                    {
                        // Since there are no moves that cause a 'three-fold' repetition, let's grab one that causes a two-fold one, hoping that the rival completes it or allows us to do so next move
                        var originalNumberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);

                        foreach (var move in bestMoveList)
                        {
                            MoveExtensions.TryParseFromUCIString(move!.Uci, allPossibleMoves, out var moveCandidate);
                            var newPosition = new Position(in position, moveCandidate!.Value);
                            var repetitions = Utils.UpdatePositionHistory(in newPosition, positionHistory);

                            // If a repetition already exists, we can't rely on two fold repetition detection,
                            // so we check if it adds any extra repetitions
                            var numberOfTwoFoldRepetitionCount = Position.NumberOfTwoFoldRepetitions(positionHistory);
                            if (numberOfTwoFoldRepetitionCount > originalNumberOfTwoFoldRepetitionCount)
                            {
                                Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                                bestMove = move;
                                break;
                            }
                            Utils.RevertPositionHistory(in newPosition, positionHistory, repetitions);
                        }

                        bestMove ??= bestMoveList.FirstOrDefault();
                    }
                }

                break;

            default:
                return (NoResult, default);
        }

        Move? parsedMove = 0;
        if (bestMove?.Uci is not null && !MoveExtensions.TryParseFromUCIString(bestMove.Uci, position.AllPossibleMoves(), out parsedMove))
        {
            throw new AssertException($"{bestMove.Uci} should be parsable from position {fen}");
        }

        return (mate, parsedMove ?? 0);
    }

    public static int EvaluationSearch(Position position, int halfMovesWithoutCaptureOrPawnMove, CancellationToken cancellationToken)
    {
        if (!Configuration.EngineSettings.UseOnlineTablebaseInSearch || position.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return NoResult;
        }

        var fen = position.FEN(halfMovesWithoutCaptureOrPawnMove);
        _logger.Debug("[{0}] Querying online tb for position {1}", nameof(EvaluationSearch), fen);

        var result = GetEvaluation(fen, cancellationToken).Result;

#pragma warning disable S3358 // Ternary operators should not be nested
        return result?.Category switch
        {
            TablebaseEvaluationCategory.Unknown => NoResult,
            TablebaseEvaluationCategory.Draw => 0,
            TablebaseEvaluationCategory.BlessedLoss => 0,
            TablebaseEvaluationCategory.CursedWin => 0,
            TablebaseEvaluationCategory.Win or TablebaseEvaluationCategory.MaybeWin =>
                Math.Abs(result.DistanceToZero ?? 0) + halfMovesWithoutCaptureOrPawnMove > 100
                    ? 0
                    : result.DistanceToMate.HasValue
                        ? EvaluationConstants.CheckMateBaseEvaluation - (EvaluationConstants.CheckmateDepthFactor * (int)Math.Ceiling(0.5 * Math.Abs(result.DistanceToMate.Value)))
                    : EvaluationConstants.CheckMateBaseEvaluation - 49 * EvaluationConstants.CheckmateDepthFactor,
            TablebaseEvaluationCategory.Loss or TablebaseEvaluationCategory.MaybeLoss =>
                Math.Abs(result.DistanceToZero ?? 0) + halfMovesWithoutCaptureOrPawnMove > 100
                    ? 0
                    : result.DistanceToMate.HasValue
                        ? -EvaluationConstants.CheckMateBaseEvaluation + (EvaluationConstants.CheckmateDepthFactor * (int)Math.Ceiling(0.5 * Math.Abs(result.DistanceToMate.Value)))
                        : -EvaluationConstants.CheckMateBaseEvaluation + 49 * EvaluationConstants.CheckmateDepthFactor,
            _ => NoResult
        };
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    private static async Task<TablebaseEvaluation?> GetEvaluation(string fen, CancellationToken cancellationToken)
    {
        try
        {
            //var response = await _retryPolicy.ExecuteAsync(async (_) => await _client.GetAsync($"standard?fen={fen}", cancellationToken), cancellationToken);
            //return JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(cancellationToken), typeof(TablebaseEvaluation), _serializerOptions);

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code - TypeInfoResolver included in _serializerOptions
            return await _client.GetFromJsonAsync<TablebaseEvaluation>($"standard?fen={fen}", _serializerOptions, cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error querying tablebase for evaluation of position {fen}", fen);
            return null;
        }
    }
}
