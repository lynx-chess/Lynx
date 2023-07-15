using NLog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Lynx.Model;
using System.Net.Http.Json;
using Polly.Extensions.Http;
using System.Net;
using Polly;
using System.Threading;
using Polly.Retry;

namespace Lynx;

public static class OnlineTablebaseProber
{
    public const int NoResult = 6666;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, 6 + retryAttempt)));    // 128, 256, 512, 1024ms


    static OnlineTablebaseProber()
    {
        var po = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, 6 + retryAttempt)));    // 128, 256, 512, 1024ms

    }
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

        switch (tablebaseEval.Category)
        {
            case TablebaseEvaluationCategory.Unknown:
                return (NoResult, default);

            // No moves in mainline endpoint if there's a draw
            case TablebaseEvaluationCategory.Draw:
                {
                    if (tablebaseEval.IsStalemate)
                    {
                        return (NoResult, default);
                    }

                    var bestMove = tablebaseEval.Moves?.FirstOrDefault(m => m.Category == TablebaseEvaluationCategory.Draw);

                    Move? parsedMove = 0;
                    if (bestMove?.Uci is not null && !MoveExtensions.TryParseFromUCIString(bestMove.Uci, position.AllPossibleMoves(), out parsedMove))
                    {
                        throw new AssertException($"{bestMove.Uci} should be parsable from position {fen}");
                    }

                    return (0, parsedMove ?? 0);
                }
            case TablebaseEvaluationCategory.Win:
            case TablebaseEvaluationCategory.Loss:
            case TablebaseEvaluationCategory.CursedWin:
            case TablebaseEvaluationCategory.BlessedLoss:
                {
                    var mate = (int)Math.Ceiling(0.5 * tablebaseEval.DistanceToMate);

                    if (tablebaseEval.DistanceToZero - halfMovesWithoutCaptureOrPawnMove > 100 || tablebaseEval.Category == TablebaseEvaluationCategory.CursedWin || tablebaseEval.Category == TablebaseEvaluationCategory.BlessedLoss)
                    {
                        _logger.Info("Cursed win or blessed loss: {0}", tablebaseEval.Category);
                        // We don't set mate to 0 since we don't really care about it due to being root node search: let it play the best moves anyway
                    }

                    var bestMove = tablebaseEval.Category switch
                    {
                        TablebaseEvaluationCategory.Win => tablebaseEval.Moves
                                                                ?.Where(m => m.Category == TablebaseEvaluationCategory.Loss)
                                                                .OrderBy(m => Math.Abs(m.DistanceToZero))
                                                                .ThenBy(m => Math.Abs(m.DistanceToMate))
                                                                .FirstOrDefault(),
                        TablebaseEvaluationCategory.Loss => tablebaseEval.Moves
                                                                ?.Where(m => m.Category == TablebaseEvaluationCategory.Win)
                                                                .OrderByDescending(m => Math.Abs(m.DistanceToZero))
                                                                .ThenByDescending(m => Math.Abs(m.DistanceToMate))
                                                                .FirstOrDefault(),
                        TablebaseEvaluationCategory.CursedWin => tablebaseEval.Moves
                                                                ?.Where(m => m.Category == TablebaseEvaluationCategory.BlessedLoss)
                                                                .OrderBy(m => Math.Abs(m.DistanceToZero))
                                                                .ThenBy(m => Math.Abs(m.DistanceToMate))
                                                                .FirstOrDefault(),
                        TablebaseEvaluationCategory.BlessedLoss => tablebaseEval.Moves
                                                                ?.Where(m => m.Category == TablebaseEvaluationCategory.CursedWin)
                                                                .OrderByDescending(m => Math.Abs(m.DistanceToZero))
                                                                .ThenByDescending(m => Math.Abs(m.DistanceToMate))
                                                                .FirstOrDefault(),
                        _ => throw new NotImplementedException()
                    };

                    Move? parsedMove = 0;
                    if (bestMove?.Uci is not null
                        && !MoveExtensions.TryParseFromUCIString(bestMove.Uci, position.AllPossibleMoves(), out parsedMove))
                    {
                        throw new AssertException($"{bestMove.Uci} should be parsable from position {fen}");
                    }

                    return (mate, parsedMove ?? 0);
                }
            default:
                throw new NotImplementedException();
        }
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
            var response = await RetryPolicy.ExecuteAsync(async (_) => await _client.GetAsync($"standard?fen={fen}", cancellationToken), cancellationToken);
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

    //    /// <summary>
    //    /// Cursed win: moves here
    //    /// Draw: no moves here
    //    /// </summary>
    //    /// <param name="fen"></param>
    //    /// <param name="cancellationToken"></param>
    //    /// <returns></returns>
    //    public static async Task<TablebaseMainLine?> GetMainLine(string fen, CancellationToken cancellationToken = default)
    //    {
    //        try
    //        {
    //#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    //            return await _client.GetFromJsonAsync<TablebaseMainLine>($"standard/mainline?fen={fen}", _options, cancellationToken);
    //#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.Error(e, "Error querying tablebase for main line of position {fen}", fen);
    //            Console.WriteLine(e.Message + "Error querying tablebase for main line of position {fen}");
    //            return null;
    //        }
    //    }
}
