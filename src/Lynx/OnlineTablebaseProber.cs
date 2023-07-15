using NLog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Lynx.Model;
using Polly.Extensions.Http;
using System.Net;
using Polly;
using Polly.Retry;

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

    private readonly static HttpClient _client = new() { BaseAddress = new("http://tablebase.lichess.ovh/") };

    private readonly static JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumMemberConverter() }
    };

    public static (int DistanceToMate, Move BestMove) RootSearch(Position position, int halfMovesWithoutCaptureOrPawnMove, CancellationToken cancellationToken)
    {
        if (!Configuration.EngineSettings.UseOnlineTablebase || position.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return (NoResult, default);
        }

        var fen = position.FEN();
        _logger.Debug("Querying online tablebase for position {0}", fen);

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
            case TablebaseEvaluationCategory.Win:
                mate = (int)Math.Ceiling(0.5 * tablebaseEval.DistanceToMate);

                if (tablebaseEval.DistanceToZero - halfMovesWithoutCaptureOrPawnMove > 100)
                {
                    _logger.Info("Cursed win due to already {0} moves without captures/pawn moves {1}", halfMovesWithoutCaptureOrPawnMove, fen);
                    // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                }

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Loss)
                    .OrderBy(m => Math.Abs(m.DistanceToZero))
                    .ThenBy(m => Math.Abs(m.DistanceToMate))
                    .FirstOrDefault();

                break;
            case TablebaseEvaluationCategory.Loss:
                mate = (int)Math.Ceiling(0.5 * tablebaseEval.DistanceToMate);

                if (tablebaseEval.DistanceToZero - halfMovesWithoutCaptureOrPawnMove > 100)
                {
                    _logger.Info("Blessed loss due to already {0} moves without captures/pawn moves {1}", halfMovesWithoutCaptureOrPawnMove, fen);
                    // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                }

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.Win)
                    .OrderByDescending(m => Math.Abs(m.DistanceToZero))
                    .ThenByDescending(m => Math.Abs(m.DistanceToMate))
                    .FirstOrDefault();

                break;
            case TablebaseEvaluationCategory.CursedWin:
                // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                mate = (int)Math.Ceiling(0.5 * tablebaseEval.DistanceToMate);

                _logger.Info("Cursed win {0}", fen);

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.BlessedLoss)
                    .OrderBy(m => Math.Abs(m.DistanceToZero))
                    .ThenBy(m => Math.Abs(m.DistanceToMate))
                    .FirstOrDefault();

                break;
            case TablebaseEvaluationCategory.BlessedLoss:
                // We don't set mate to 0 since we don't really care about it due to being root node search, and we have to play the best moves anyway
                mate = (int)Math.Ceiling(0.5 * tablebaseEval.DistanceToMate);

                _logger.Info("Blessed loss {0}", fen);

                bestMove = tablebaseEval.Moves
                    ?.Where(m => m.Category == TablebaseEvaluationCategory.CursedWin)
                    .OrderByDescending(m => Math.Abs(m.DistanceToZero))
                    .ThenByDescending(m => Math.Abs(m.DistanceToMate))
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
        if (!Configuration.EngineSettings.UseOnlineTablebase || position.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return NoResult;
        }

        var result = GetEvaluation(position.FEN(), cancellationToken).Result;

        if (result?.Category > TablebaseEvaluationCategory.Unknown)
        {
            return result.Category switch
            {
                TablebaseEvaluationCategory.Unknown => NoResult,
                TablebaseEvaluationCategory.Draw => 0,
                TablebaseEvaluationCategory.BlessedLoss => 0,
                TablebaseEvaluationCategory.CursedWin => 0,
                _ => result.DistanceToMate + halfMovesWithoutCaptureOrPawnMove > 100
                        ? 0
                        : Math.Sign(result.DistanceToMate) * (EvaluationConstants.CheckMateEvaluation - (Position.DepthFactor * (int)Math.Ceiling(0.5 * Math.Abs(result.DistanceToMate)))) * (position.Side == Side.White ? 1 : -1)
            };
        }

        return NoResult;
    }

    public static async Task<TablebaseEvaluation?> GetEvaluation(string fen, CancellationToken cancellationToken)
    {
        try
        {
            //return await _client.GetFromJsonAsync<TablebaseEvaluation>($"standard?fen={fen}", _options, cancellationToken);
            var response = await _retryPolicy.ExecuteAsync(async (_) => await _client.GetAsync($"standard?fen={fen}", cancellationToken), cancellationToken);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            return JsonSerializer.Deserialize<TablebaseEvaluation>(await response.Content.ReadAsStringAsync(cancellationToken), _options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error querying tablebase for evaluation of position {fen}", fen);
            Console.WriteLine(e.Message + "Error querying tablebase for evaluation of position {fen}");
            return null;
        }
    }
}
