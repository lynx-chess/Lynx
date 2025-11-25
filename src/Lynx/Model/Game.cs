using Lynx.UCI.Commands.GUI;
using NLog;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public sealed class Game : IDisposable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
#pragma warning disable CA1002 // Do not expose generic lists
    public List<Move> MoveHistory { get; }
#pragma warning restore CA1002 // Do not expose generic lists
#endif

    //private int _positionHashHistoryPointerBeforeLastSearch;
    private int _positionHashHistoryPointer;
    private readonly ulong[] _positionHashHistory;

    /// <summary>
    /// Indexed by ply
    /// </summary>
    private readonly PlyStackEntry[] _stack;

    private bool _disposedValue;

    public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

    public Position CurrentPosition { get; private set; }

    public Position PositionBeforeLastSearch { get; private set; }

    public string FEN => CurrentPosition.FEN(HalfMovesWithoutCaptureOrPawnMove);

    private Game()
    {
        _positionHashHistory = ArrayPool<ulong>.Shared.Rent(Constants.MaxNumberMovesInAGame);
        _stack = ArrayPool<PlyStackEntry>.Shared.Rent(Constants.MaxNumberMovesInAGame + EvaluationConstants.ContinuationHistoryPlyCount);

        CurrentPosition = new Position(Constants.InitialPositionFEN);
        PositionBeforeLastSearch = new Position(CurrentPosition);

#if DEBUG
        MoveHistory = new(Constants.MaxNumberMovesInAGame);
#endif
    }

    public Game(string fen)
        : this()
    {
        ParseFEN(fen);
    }

    public void ParseFEN(ReadOnlySpan<char> fen)
    {
        Populate(fen, []);
    }

    public void ParsePositionCommand(ReadOnlySpan<char> positionCommandSpan)
    {
        try
        {

            // We divide the position command in these two sections:
            // "position startpos                       ||"
            // "position startpos                       || moves e2e4 e7e5"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
            Span<Range> items = stackalloc Range[2];
            positionCommandSpan.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var initialPositionSection = positionCommandSpan[items[0]];

            // We divide in these two parts
            // "position startpos ||"       <-- If "fen" doesn't exist in the section
            // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
            Span<Range> initialPositionParts = stackalloc Range[2];
            initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == PositionCommand.Id.Length   // "position" or "position startpos"
                ? initialPositionSection[initialPositionParts[1]]
                : Constants.InitialPositionFEN.AsSpan();

            var movesSection = positionCommandSpan[items[1]];

            Populate(fen, movesSection);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error parsing position command '{0}'", positionCommandSpan.ToString());
            Populate(Constants.InitialPositionFEN, []);
        }
    }

    private void Populate(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves)
    {
        var parsedFen = FENParser.ParseFEN(fen);
        CurrentPosition.PopulateFrom(parsedFen);

        if (!CurrentPosition.IsValid())
        {
            _logger.Warn("Invalid position detected: {FEN}", fen.ToString());
        }

        AddToPositionHashHistory(CurrentPosition.UniqueIdentifier);
        HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        // Number of potential half-moves provided in the string
        Span<Range> moveRanges = stackalloc Range[(rawMoves.Length / 5) + 1];
        rawMoves.Split(moveRanges, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (int i = 0; i < moveRanges.Length; ++i)
        {
            if (moveRanges[i].Start.Equals(moveRanges[i].End))
            {
                break;
            }

            var moveString = rawMoves[moveRanges[i]];
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, ref evaluationContext, movePool);

            // TODO: consider creating moves on the fly
            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                break;
            }

            MakeMove(parsedMove.Value);
        }

        PositionBeforeLastSearch.ResetTo(CurrentPosition);
        //_positionHashHistoryPointerBeforeLastSearch = _positionHashHistoryPointer;
    }

    /// <summary>
    /// Updates <paramref name="halfMovesWithoutCaptureOrPawnMove"/>.
    /// See also <see cref="Utils.Update50movesRule(int, int)"/>
    /// </summary>
    /// <remarks>
    /// Checking halfMovesWithoutCaptureOrPawnMove >= 100 since a capture/pawn move doesn't necessarily 'clear' the variable.
    /// i.e. while the engine is searching:
    ///     At depth 2, 50 rules move applied and eval is 0
    ///     At depth 3, there's a capture, but the eval should still be 0
    ///     At depth 4 there's no capture, but the eval should still be 0
    /// </remarks>
    /// <returns>true if threefol/50 moves repetition is possible (since both captures and pawn moves are irreversible)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Update50movesRule(Move moveToPlay)
    {
        var isCapture = moveToPlay.CapturedPiece() != (int)Piece.None;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThreefoldRepetition()
    {
        var currentHash = CurrentPosition.UniqueIdentifier;

        // [_positionHashHistoryPointer - 1] would be the last one, we want to start searching 2 ealier and finish HalfMovesWithoutCaptureOrPawnMove earlier
        var limit = Math.Max(0, _positionHashHistoryPointer - 1 - HalfMovesWithoutCaptureOrPawnMove);
        for (int i = _positionHashHistoryPointer - 3; i >= limit; i -= 2)
        {
            if (currentHash == _positionHashHistory[i])
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Is50MovesRepetition(ref EvaluationContext evaluationContext)
    {
        if (HalfMovesWithoutCaptureOrPawnMove < 100)
        {
            return false;
        }

        return !CurrentPosition.IsInCheck() || MoveGenerator.CanGenerateAtLeastAValidMove(CurrentPosition, ref evaluationContext);
    }

    /// <summary>
    /// To be used in online tb proving only, in combination with the result of <see cref="CopyPositionHashHistory"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsThreefoldRepetition(ReadOnlySpan<ulong> positionHashHistory, Position position, int halfMovesWithoutCaptureOrPawnMove = Constants.MaxNumberMovesInAGame)
    {
        var currentHash = position.UniqueIdentifier;

        // Since positionHashHistory hasn't been updated with position, [Count] would be the last one, so we want to start searching 2 ealier
        var limit = Math.Max(0, positionHashHistory.Length - halfMovesWithoutCaptureOrPawnMove);
        for (int i = positionHashHistory.Length - 2; i >= limit; i -= 2)
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
            AddToPositionHashHistory(CurrentPosition.UniqueIdentifier);
            Update50movesRule(moveToPlay);
        }
        else
        {
            CurrentPosition.UnmakeMove(moveToPlay, gameState);
            _logger.Warn("Error trying to play move {0} in {1}", moveToPlay.UCIString(), CurrentPosition.FEN(HalfMovesWithoutCaptureOrPawnMove));
        }

        return gameState;
    }

    /// <summary>
    /// Cleans <see cref="CurrentPosition"/> value, since in case of search cancellation
    /// (either by the engine time management logic or by external stop command)
    /// currentPosition won't be the initial one
    /// </summary>
    public void ResetCurrentPositionToBeforeSearchState()
    {
        CurrentPosition.ResetTo(PositionBeforeLastSearch);
        //_positionHashHistoryPointer = _positionHashHistoryPointerBeforeLastSearch;    // TODO
    }

    public void UpdateInitialPosition()
    {
        PositionBeforeLastSearch.ResetTo(CurrentPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateMoveinStack(int n, Move move) => _stack[n + EvaluationConstants.ContinuationHistoryPlyCount].Move = move;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Move ReadMoveFromStack(int n) => _stack[n + EvaluationConstants.ContinuationHistoryPlyCount].Move;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadStaticEvalFromStack(int n) => _stack[n + EvaluationConstants.ContinuationHistoryPlyCount].StaticEval;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int UpdateStaticEvalInStack(int n, int value) => _stack[n + EvaluationConstants.ContinuationHistoryPlyCount].StaticEval = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadDoubleExtensionsFromStack(int n) => _stack[n + EvaluationConstants.ContinuationHistoryPlyCount].DoubleExtensions;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref PlyStackEntry Stack(int n) => ref _stack[n + EvaluationConstants.ContinuationHistoryPlyCount];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PositionHashHistoryLength() => _positionHashHistoryPointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddToPositionHashHistory(ulong hash) => _positionHashHistory[_positionHashHistoryPointer++] = hash;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveFromPositionHashHistory() => --_positionHashHistoryPointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong[] CopyPositionHashHistory() => _positionHashHistory[.._positionHashHistoryPointer];

    internal void ClearPositionHashHistory() => _positionHashHistoryPointer = 0;

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ArrayPool<PlyStackEntry>.Shared.Return(_stack, clearArray: true);
                ArrayPool<ulong>.Shared.Return(_positionHashHistory);

                CurrentPosition.Dispose();
                PositionBeforeLastSearch.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);

#pragma warning disable S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors - https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        GC.SuppressFinalize(this);
#pragma warning restore S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors
    }
}
