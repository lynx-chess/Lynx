using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Buffers;

namespace Lynx.Benchmark;
public class CalculateThreats_Benchmark : BaseBenchmark
{
    private readonly Position_CalculateThreats_Benchmark[] _positions;

    public CalculateThreats_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position_CalculateThreats_Benchmark(fen))];
    }

    [Benchmark(Baseline = true)]
    public void Original()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        foreach (var position in _positions)
        {
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            position.CalculateThreats_Original(ref evaluationContext);
        }
    }

    [Benchmark]
    public void Reference()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        foreach (var position in _positions)
        {
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            position.CalculateThreats_Reference(ref evaluationContext);
        }
    }
}

class Position_CalculateThreats_Benchmark
{
    internal int _incrementalEvalAccumulator;
    internal int _incrementalPhaseAccumulator;
    internal bool _isIncrementalEval;

    private readonly ulong[] _pieceBitBoards;
    private readonly ulong[] _occupancyBitBoards;
    private readonly int[] _board;

    private readonly byte _castle;

#pragma warning disable S3887, CA1051
    private readonly byte[] _castlingRightsUpdateConstants;
    public readonly ulong[] KingsideCastlingFreeSquares;
    public readonly ulong[] KingsideCastlingNonAttackedSquares;
    public readonly ulong[] QueensideCastlingFreeSquares;
    public readonly ulong[] QueensideCastlingNonAttackedSquares;

#pragma warning disable IDE1006 // Naming Styles
    internal readonly int WhiteShortCastle;
    internal readonly int WhiteLongCastle;
    internal readonly int BlackShortCastle;
    internal readonly int BlackLongCastle;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore S3887, CA1051

    private readonly BoardSquare _enPassant;
    private readonly Side _side;

#pragma warning disable RCS1085 // Use auto-implemented property

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards => _pieceBitBoards;

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards => _occupancyBitBoards;

    /// <summary>
    /// Piece location indexed by square
    /// </summary>
    public int[] Board => _board;

    public Side Side => _side;

    public BoardSquare EnPassant => _enPassant;

    /// <summary>
    /// See <see cref="<CastlingRights"/>
    /// </summary>
    public byte Castle { get => _castle; }

#pragma warning restore RCS1085 // Use auto-implemented property

    public BitBoard Queens => _pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q];
    public BitBoard Rooks => _pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r];
    public BitBoard Bishops => _pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b];
    public BitBoard Knights => _pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n];
    public BitBoard Kings => _pieceBitBoards[(int)Piece.K] | _pieceBitBoards[(int)Piece.k];

    public int WhiteKingSquare => _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
    public int BlackKingSquare => _pieceBitBoards[(int)Piece.k].GetLS1BIndex();

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    public Position_CalculateThreats_Benchmark(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position_CalculateThreats_Benchmark(ParseFENResult parsedFEN)
    {
        _pieceBitBoards = parsedFEN.PieceBitBoards;
        _occupancyBitBoards = parsedFEN.OccupancyBitBoards;
        _board = parsedFEN.Board;

        _side = parsedFEN.Side;
        _castle = parsedFEN.Castle;
        _enPassant = parsedFEN.EnPassant;

        _isIncrementalEval = false;

        _castlingRightsUpdateConstants = ArrayPool<byte>.Shared.Rent(64);
        Array.Fill(_castlingRightsUpdateConstants, Constants.NoUpdateCastlingRight, 0, 64);

        // It won't be possible to add castling rights to a position created froma FEN without them
        if (_castle == (int)CastlingRights.None)
        {
            KingsideCastlingFreeSquares = [];
            QueensideCastlingFreeSquares = [];
            KingsideCastlingNonAttackedSquares = [];
            QueensideCastlingNonAttackedSquares = [];
        }
        else
        {
            var whiteKingSquare = WhiteKingSquare;
            var blackKingSquare = BlackKingSquare;

            _castlingRightsUpdateConstants[whiteKingSquare] = Constants.WhiteKingCastlingRight;
            _castlingRightsUpdateConstants[blackKingSquare] = Constants.BlackKingCastlingRight;

            var castlingData = parsedFEN.CastlingData;

            var whiteKingsideRook = castlingData.WhiteKingsideRook;
            if (whiteKingsideRook != CastlingData.DefaultValues)
            {
                _castlingRightsUpdateConstants[whiteKingsideRook] = Constants.WhiteKingSideRookCastlingRight;
            }

            var whiteQueensideRook = castlingData.WhiteQueensideRook;
            if (whiteQueensideRook != CastlingData.DefaultValues)
            {
                _castlingRightsUpdateConstants[whiteQueensideRook] = Constants.WhiteQueenSideRookCastlingRight;
            }

            var blackKingsideRook = castlingData.BlackKingsideRook;
            if (blackKingsideRook != CastlingData.DefaultValues)
            {
                _castlingRightsUpdateConstants[blackKingsideRook] = Constants.BlackKingSideRookCastlingRight;
            }

            var blackQueensideRook = castlingData.BlackQueensideRook;
            if (blackQueensideRook != CastlingData.DefaultValues)
            {
                _castlingRightsUpdateConstants[blackQueensideRook] = Constants.BlackQueenSideRookCastlingRight;
            }

            KingsideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            KingsideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingShortCastleSquare);
            KingsideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingShortCastleSquare);

            QueensideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            QueensideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingLongCastleSquare);
            QueensideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingLongCastleSquare);

            // This could be simplified/harcoded for standard chess, see FreeAndNonAttackedSquares
            KingsideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);

            var whiteKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.White]
                | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingsideRook, Constants.WhiteRookShortCastleSquare);
            whiteKingsideFreeMask.PopBit(whiteKingSquare);
            whiteKingsideFreeMask.PopBit(whiteKingsideRook);

            var blackKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.Black]
                | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingsideRook, Constants.BlackRookShortCastleSquare);
            blackKingsideFreeMask.PopBit(blackKingSquare);
            blackKingsideFreeMask.PopBit(blackKingsideRook);

            KingsideCastlingFreeSquares[(int)Side.White] = whiteKingsideFreeMask;
            KingsideCastlingFreeSquares[(int)Side.Black] = blackKingsideFreeMask;

            // This could be simplified/harcoded for standard chess, see FreeAndNonAttackedSquares
            QueensideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);

            var whiteQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.White]
                | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteQueensideRook, Constants.WhiteRookLongCastleSquare);
            whiteQueensideFreeMask.PopBit(whiteKingSquare);
            whiteQueensideFreeMask.PopBit(whiteQueensideRook);

            var blackQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.Black]
                | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackQueensideRook, Constants.BlackRookLongCastleSquare);
            blackQueensideFreeMask.PopBit(blackKingSquare);
            blackQueensideFreeMask.PopBit(blackQueensideRook);

            QueensideCastlingFreeSquares[(int)Side.White] = whiteQueensideFreeMask;
            QueensideCastlingFreeSquares[(int)Side.Black] = blackQueensideFreeMask;

            // Usual encoding for standard chess, King to target square
            if (!Configuration.EngineSettings.IsChess960)
            {
                WhiteShortCastle = MoveExtensions.EncodeShortCastle(whiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K);
                WhiteLongCastle = MoveExtensions.EncodeLongCastle(whiteKingSquare, Constants.WhiteKingLongCastleSquare, (int)Piece.K);

                BlackShortCastle = MoveExtensions.EncodeShortCastle(blackKingSquare, Constants.BlackKingShortCastleSquare, (int)Piece.k);
                BlackLongCastle = MoveExtensions.EncodeLongCastle(blackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k);
            }
            // KxR encoding for DFRC
            else
            {
                WhiteShortCastle = MoveExtensions.EncodeShortCastle(whiteKingSquare, whiteKingsideRook, (int)Piece.K);
                WhiteLongCastle = MoveExtensions.EncodeLongCastle(whiteKingSquare, whiteQueensideRook, (int)Piece.K);

                BlackShortCastle = MoveExtensions.EncodeShortCastle(blackKingSquare, blackKingsideRook, (int)Piece.k);
                BlackLongCastle = MoveExtensions.EncodeLongCastle(blackKingSquare, blackQueensideRook, (int)Piece.k);
            }
        }
    }

    public EvaluationContext CalculateThreats_Original(ref EvaluationContext evaluationContext)
    {
        var occupancy = OccupancyBitBoards[(int)Side.Both];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = PieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                evaluationContext.Attacks[pieceIndex] |= attacks(square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.White] |= evaluationContext.Attacks[pieceIndex];
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = PieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                evaluationContext.Attacks[pieceIndex] |= attacks(square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.Black] |= evaluationContext.Attacks[pieceIndex];
        }

        return evaluationContext;
    }

    public EvaluationContext CalculateThreats_Reference(ref EvaluationContext evaluationContext)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];
        ref var attacksByWhite = ref evaluationContext.AttacksBySide[(int)Side.White];
        ref var attacksByBlack = ref evaluationContext.AttacksBySide[(int)Side.Black];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            attacksByWhite |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            attacksByBlack |= existingAttacks;
        }

        return evaluationContext;
    }
}
