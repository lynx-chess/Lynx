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

public static class OnlineTablebaseProber
{
    public const int NoResult = 6666;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly static AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, 6 + retryAttempt)));    // 128, 256, 512, 1024ms

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

    public static (int DistanceToMate, Move BestMove) RootSearch(Position position, int halfMovesWithoutCaptureOrPawnMove, CancellationToken cancellationToken)
    {
        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || position.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return (NoResult, default);
        }

        var fen = position.FEN();
        _logger.Debug("[{0}] Querying online tb for position {1}", nameof(RootSearch), fen);

        var tablebaseEval = GetEvaluation(fen, cancellationToken).Result;

        if (tablebaseEval is null || tablebaseEval.Category == TablebaseEvaluationCategory.Unknown)
        {
            _logger.Warn("An answer was expected from online tablebase for position {0}", fen);
            return (NoResult, default);
        }

        TablebaseEvalMove? bestMove;
        int mate = 0;

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
                    mate = (int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                    Math.CopySign(mate, tablebaseEval.DistanceToMate.Value);
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

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Loss)
                    .OrderBy(m => Math.Abs(m.DistanceToZero ?? 0))
                    .ThenBy(m => Math.Abs(m.DistanceToMate ?? 0))
                    .FirstOrDefault();

                break;
            case TablebaseEvaluationCategory.Loss:
            case TablebaseEvaluationCategory.MaybeLoss:
                if (tablebaseEval.DistanceToMate.HasValue)
                {
                    mate = (int)Math.Ceiling(0.5 * Math.Abs(tablebaseEval.DistanceToMate.Value));
                    Math.CopySign(mate, tablebaseEval.DistanceToMate.Value);
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

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Win)
                    .OrderByDescending(m => Math.Abs(m.DistanceToZero ?? 0))
                    .ThenByDescending(m => Math.Abs(m.DistanceToMate ?? 0))
                    .FirstOrDefault();

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

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.BlessedLoss)
                    .OrderBy(m => Math.Abs(m.DistanceToZero ?? 0))
                    .ThenBy(m => Math.Abs(m.DistanceToMate ?? 0))
                    .FirstOrDefault();

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

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.CursedWin)
                    .OrderByDescending(m => Math.Abs(m.DistanceToZero ?? 0))
                    .ThenByDescending(m => Math.Abs(m.DistanceToMate ?? 0))
                    .FirstOrDefault();

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

        var fen = position.FEN();
        _logger.Debug("[{0}] Querying online tb for position {1}", nameof(EvaluationSearch), fen);

        var result = GetEvaluation(position.FEN(), cancellationToken).Result;

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
                        ? EvaluationConstants.CheckMateEvaluation - (Position.DepthFactor * (int)Math.Ceiling(0.5 * Math.Abs(result.DistanceToMate.Value)))
                    : EvaluationConstants.CheckMateEvaluation - 49 * Position.DepthFactor,
            TablebaseEvaluationCategory.Loss or TablebaseEvaluationCategory.MaybeLoss =>
                Math.Abs(result.DistanceToZero ?? 0) + halfMovesWithoutCaptureOrPawnMove > 100
                    ? 0
                    : result.DistanceToMate.HasValue
                        ? -EvaluationConstants.CheckMateEvaluation + (Position.DepthFactor * (int)Math.Ceiling(0.5 * Math.Abs(result.DistanceToMate.Value)))
                        : -EvaluationConstants.CheckMateEvaluation + 49 * Position.DepthFactor,
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
