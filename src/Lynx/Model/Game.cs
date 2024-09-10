using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public sealed class Game
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
    public List<Move> MoveHistory { get; }
#endif

    public List<long> PositionHashHistory { get; }

    /// <summary>
    /// Indexed by ply
    /// </summary>
    private readonly Move[] _moveStack;

    public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

    public Position CurrentPosition { get; private set; }

    public Position PositionBeforeLastSearch { get; private set; }

    public Game() : this(Constants.InitialPositionFEN)
    {
    }

    public Game(ReadOnlySpan<char> fen)
    {
        var parsedFen = FENParser.ParseFEN(fen);
        CurrentPosition = new Position(parsedFen);
        PositionBeforeLastSearch = new Position(CurrentPosition);

        if (!CurrentPosition.IsValid())
        {
            _logger.Warn($"Invalid position detected: {fen.ToString()}");
        }

        PositionHashHistory = new(Constants.MaxNumberMovesInAGame) { CurrentPosition.UniqueIdentifier };
        HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
        _moveStack = new Move[1024];

#if DEBUG
        MoveHistory = new(Constants.MaxNumberMovesInAGame);
#endif
    }

    public Game(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan, Span<Move> movePool) : this(fen)
    {
        for (int i = 0; i < rangeSpan.Length; ++i)
        {
            if (rangeSpan[i].Start.Equals(rangeSpan[i].End))
            {
                break;
            }
            var moveString = rawMoves[rangeSpan[i]];
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, movePool);

            // TODO: consider creating moves on the fly
            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                break;
            }

            MakeMove(parsedMove.Value);
            var alternativeEval = CurrentPosition.InitialStaticEvaluation().PackedScore;

            if (CurrentPosition._needsFullEvalRecalculation && CurrentPosition._incrementalEvaluation != alternativeEval)
            {
                Console.WriteLine($"{CurrentPosition.FEN()} -> Incremental = {CurrentPosition._incrementalEvaluation} vs Initial = {alternativeEval}");
            }

        }

        PositionBeforeLastSearch = new Position(CurrentPosition);
    }

    /// <summary>
    /// Updates <paramref name="halfMovesWithoutCaptureOrPawnMove"/>.
    /// See also <see cref="Utils.Update50movesRule(int, int)"/>
    /// </summary>
    /// <param name="moveToPlay"></param>
    /// <param name="isCapture"></param>
    /// <remarks>
    /// Checking halfMovesWithoutCaptureOrPawnMove >= 100 since a capture/pawn move doesn't necessarily 'clear' the variable.
    /// i.e. while the engine is searching:
    ///     At depth 2, 50 rules move applied and eval is 0
    ///     At depth 3, there's a capture, but the eval should still be 0
    ///     At depth 4 there's no capture, but the eval should still be 0
    /// </remarks>
    /// <returns>true if threefol/50 moves repetition is possible (since both captures and pawn moves are irreversible)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Update50movesRule(Move moveToPlay, bool isCapture)
    {
        if (isCapture)
        {
            if (HalfMovesWithoutCaptureOrPawnMove < 100)
            {
                HalfMovesWithoutCaptureOrPawnMove = 0;
            }
            else
            {
                ++HalfMovesWithoutCaptureOrPawnMove;
            }

            return false;
        }
        else
        {
            var pieceToMove = moveToPlay.Piece();

            if (pieceToMove == (int)Piece.P || pieceToMove == (int)Piece.p)
            {
                if (HalfMovesWithoutCaptureOrPawnMove < 100)
                {
                    HalfMovesWithoutCaptureOrPawnMove = 0;
                }

                return false;
            }

            ++HalfMovesWithoutCaptureOrPawnMove;

            return true;
        }
    }

    /// <summary>
    /// Basic algorithm described in https://web.archive.org/web/20201107002606/https://marcelk.net/2013-04-06/paper/upcoming-rep-v2.pdf
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThreefoldRepetition()
    {
        var currentHash = CurrentPosition.UniqueIdentifier;

        // [Count - 1] would be the last one, we want to start searching 2 ealier and finish HalfMovesWithoutCaptureOrPawnMove earlier
        var limit = Math.Max(0, PositionHashHistory.Count - 1 - HalfMovesWithoutCaptureOrPawnMove);
        for (int i = PositionHashHistory.Count - 3; i >= limit; i -= 2)
        {
            if (currentHash == PositionHashHistory[i])
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Is50MovesRepetition()
    {
        if (HalfMovesWithoutCaptureOrPawnMove < 100)
        {
            return false;
        }

        return !CurrentPosition.IsInCheck() || MoveGenerator.CanGenerateAtLeastAValidMove(CurrentPosition);
    }

    /// <summary>
    /// To be used in online tb proving only, with a copy of <see cref="PositionHashHistory"/> that hasn't been updated with <paramref name="position"/>
    /// </summary>
    /// <param name="positionHashHistory"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsThreefoldRepetition(List<long> positionHashHistory, Position position, int halfMovesWithoutCaptureOrPawnMove = Constants.MaxNumberMovesInAGame)
    {
        var currentHash = position.UniqueIdentifier;

        // Since positionHashHistory hasn't been updated with position, [Count] would be the last one, so we want to start searching 2 ealier
        var limit = Math.Max(0, positionHashHistory.Count - halfMovesWithoutCaptureOrPawnMove);
        for (int i = positionHashHistory.Count - 2; i >= limit; i -= 2)
        {
            if (currentHash == positionHashHistory[i])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// To be used in online tb proving only, with a copy of <see cref="HalfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    /// <param name="halfMovesWithoutCaptureOrPawnMove"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is50MovesRepetition(int halfMovesWithoutCaptureOrPawnMove) => halfMovesWithoutCaptureOrPawnMove >= 100;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMove(Move moveToPlay)
    {
        var gameState = CurrentPosition.MakeMove(moveToPlay);

        if (CurrentPosition.WasProduceByAValidMove())
        {
#if DEBUG
            MoveHistory.Add(moveToPlay);
#endif
            PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
            Update50movesRule(moveToPlay, moveToPlay.IsCapture());
        }
        else
        {
            _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
            CurrentPosition.UnmakeMove(moveToPlay, gameState);
        }

        return gameState;
    }

    /// <summary>
    /// Cleans <see cref="CurrentPosition"/> value, since in case of search cancellation
    /// (either by the engine time management logic or by external stop command)
    /// currentPosition won't be the initial one
    /// </summary>
    public void ResetCurrentPositionToBeforeSearchState() => CurrentPosition = new(PositionBeforeLastSearch);

    public void UpdateInitialPosition() => PositionBeforeLastSearch = new(CurrentPosition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushToMoveStack(int n, Move move) => _moveStack[n + EvaluationConstants.ContinuationHistoryPlyCount] = move;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Move PopFromMoveStack(int n) => _moveStack[n + EvaluationConstants.ContinuationHistoryPlyCount];
}
