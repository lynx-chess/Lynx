/*
 * Consistent local reults
 *
 *  |                        Method |                 data |      Mean |     Error |    StdDev | Ratio | RatioSD |        Gen0 |  Allocated | Alloc Ratio |
 *  |------------------------------ |--------------------- |----------:|----------:|----------:|------:|--------:|------------:|-----------:|------------:|
 *  |                   NewPosition | (2K2r(...)1, 6) [38] | 769.76 ms | 15.348 ms | 21.009 ms |  1.00 |    0.00 | 522000.0000 | 1042.91 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (2K2r(...)1, 6) [38] | 701.44 ms | 13.447 ms | 16.008 ms |  0.91 |    0.03 | 147000.0000 |  293.38 MB |        0.28 |
 *  | MakeUnmakeMove_WithZobristKey | (2K2r(...)1, 6) [38] | 585.71 ms | 11.135 ms | 10.416 ms |  0.76 |    0.02 | 147000.0000 |  293.38 MB |        0.28 |
 *  |                               |                      |           |           |           |       |         |             |            |
 *      |
 *  |                   NewPosition | (3K4/(...)1, 6) [38] | 728.22 ms | 14.007 ms | 18.213 ms |  1.00 |    0.00 | 522000.0000 | 1042.91 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (3K4/(...)1, 6) [38] | 705.43 ms | 13.556 ms | 17.144 ms |  0.97 |    0.04 | 147000.0000 |  293.38 MB |        0.28 |
 *  | MakeUnmakeMove_WithZobristKey | (3K4/(...)1, 6) [38] | 575.41 ms | 11.332 ms | 15.511 ms |  0.79 |    0.02 | 147000.0000 |  293.38 MB |        0.28 |
 *  |                               |                      |           |           |           |       |         |             |            |
 *      |
 *  |                   NewPosition | (8/p7(...)-, 6) [37] |  26.66 ms |  0.524 ms |  0.735 ms |  1.00 |    0.00 |  21000.0000 |   41.91 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (8/p7(...)-, 6) [37] |  25.73 ms |  0.395 ms |  0.350 ms |  0.96 |    0.03 |   8375.0000 |   16.72 MB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey | (8/p7(...)-, 6) [37] |  22.08 ms |  0.416 ms |  0.408 ms |  0.83 |    0.04 |   8375.0000 |   16.72 MB |        0.40 |
 *  |                               |                      |           |           |           |       |         |             |            |
 *      |
 *  |                   NewPosition | (r3k2(...)1, 4) [73] | 728.79 ms | 11.894 ms | 10.544 ms |  1.00 |    0.00 | 392000.0000 |  782.46 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (r3k2(...)1, 4) [73] | 777.59 ms | 15.437 ms | 20.072 ms |  1.06 |    0.02 |  51000.0000 |  102.19 MB |        0.13 |
 *  | MakeUnmakeMove_WithZobristKey | (r3k2(...)1, 4) [73] | 567.77 ms | 11.083 ms | 14.016 ms |  0.78 |    0.02 |  51000.0000 |  102.19 MB |        0.13 |
 *  |                               |                      |           |           |           |       |         |             |            |
 *      |
 *  |                   NewPosition | (r4rk(...)0, 4) [77] | 601.53 ms | 11.690 ms | 14.784 ms |  1.00 |    0.00 | 374000.0000 |  747.62 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (r4rk(...)0, 4) [77] | 606.93 ms | 11.651 ms | 13.870 ms |  1.01 |    0.03 |  47000.0000 |   94.08 MB |        0.13 |
 *  | MakeUnmakeMove_WithZobristKey | (r4rk(...)0, 4) [77] | 474.53 ms |  9.236 ms | 12.948 ms |  0.79 |    0.03 |  47000.0000 |   94.08 MB |        0.13 |
 *  |                               |                      |           |           |           |       |         |             |            |
 *      |
 *  |                   NewPosition | (rnbq(...)1, 4) [61] |  38.23 ms |  0.739 ms |  1.059 ms |  1.00 |    0.00 |  21384.6154 |   42.71 MB |        1.00 |
 *  |       MakeUnmakeMove_Original | (rnbq(...)1, 4) [61] |  39.44 ms |  0.778 ms |  1.500 ms |  1.04 |    0.05 |   4769.2308 |    9.54 MB |        0.22 |
 *  | MakeUnmakeMove_WithZobristKey | (rnbq(...)1, 4) [61] |  29.83 ms |  0.586 ms |  0.651 ms |  0.78 |    0.03 |   4781.2500 |    9.54 MB |        0.22 |
 */

#pragma warning disable S101, S1854 // Types should be named in PascalCase

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class MakeUnmakeMove_implementation : BaseBenchmark
{
    public static IEnumerable<(string, int)> Data => new[] {
            (Constants.InitialPositionFEN, 4),
            (Constants.TrickyTestPositionFEN, 4),
            ("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 4),
            ("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1", 6),
            ("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1", 6),
            ("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -", 6),
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long NewPosition((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_Original(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_Original((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_WithZobristKey((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_WithZobritsKey(new(data.Fen), data.Depth, default);

    public static class MakeMovePerft
    {
        public static long ResultsImpl_Original(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    var newPosition = new MakeMovePosition(position, move);

                    if (newPosition.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_Original(newPosition, depth - 1, nodes);
                    }
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_Original(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_Original(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove_WithZobritsKey(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_WithZobristKey(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_WithZobritsKey(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_WithZobristKey(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }
    }

    public struct MakeMovePosition
    {
        public long UniqueIdentifier { get; private set; }

        public BitBoard[] PieceBitBoards { get; }

        public BitBoard[] OccupancyBitBoards { get; }

        public Side Side { get; private set; }

        public BoardSquare EnPassant { get; private set; }

        public int Castle { get; private set; }

        public MakeMovePosition(string fen) : this(FENParser.ParseFEN(fen))
        {
        }

        public MakeMovePosition((BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, int Castle, BoardSquare EnPassant,
            int HalfMoveClock/*, int FullMoveCounter*/) parsedFEN)
        {
            PieceBitBoards = parsedFEN.PieceBitBoards;
            OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
            Side = parsedFEN.Side;
            Castle = parsedFEN.Castle;
            EnPassant = parsedFEN.EnPassant;

            UniqueIdentifier = MakeMoveZobristTable.PositionHash(this);
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMovePosition(MakeMovePosition position)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        /// <summary>
        /// Null moves constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nullMove"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
        public MakeMovePosition(MakeMovePosition position, bool nullMove)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = (Side)Utils.OppositeSide(position.Side);
            Castle = position.Castle;
            EnPassant = BoardSquare.noSquare;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)position.EnPassant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMovePosition(MakeMovePosition position, Move move) : this(position)
        {
            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            EnPassant = BoardSquare.noSquare;

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)position.EnPassant)
                ^ ZobristTable.CastleHash(position.Castle);

            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameState MakeMove_Original(Move move)
        {
            int capturedPiece = -1;
            int castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;

            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                    capturedPiece = oppositePawnIndex;
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            capturedPiece = pieceIndex;
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameState(capturedPiece, castleCopy, enpassantCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_Original(Move move, MakeMoveGameState gameState)
        {
            var oppositeSide = (int)Side;
            Side = (Side)Utils.OppositeSide(Side);
            var offset = Utils.PieceOffset(Side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[(int)Side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[(int)Side].SetBit(sourceSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)        // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                  // We clear the existing castling rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                }
                else
                {
                    PieceBitBoards[gameState.CapturedPiece].SetBit(targetSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, gameState.CapturedPiece);

                    OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                EnPassant = gameState.EnPassant;
                UniqueIdentifier ^= ZobristTable.EnPassantHash((int)EnPassant);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle = gameState.Castle;

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameStateWithZobristKey MakeMove_WithZobristKey(Move move)
        {
            int capturedPiece = -1;
            int castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;
            long uniqueIdentifierCopy = UniqueIdentifier;

            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                    capturedPiece = oppositePawnIndex;
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            capturedPiece = pieceIndex;
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameStateWithZobristKey(capturedPiece, castleCopy, enpassantCopy, uniqueIdentifierCopy);
            //var clone = new Position(this);
            //clone.UnmakeMove(move, gameState);
            //if (uniqueIdentifierCopy != clone.UniqueIdentifier)
            //{
            //    throw new($"{FEN()}: {uniqueIdentifierCopy} expected, got {clone.UniqueIdentifier} got after Make/Unmake move {move.ToEPDString()}");
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_WithZobristKey(Move move, MakeMoveGameStateWithZobristKey gameState)
        {
            var oppositeSide = (int)Side;
            Side = (Side)Utils.OppositeSide(Side);
            var offset = Utils.PieceOffset(Side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[(int)Side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[(int)Side].SetBit(sourceSquare);

            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                }
                else
                {
                    PieceBitBoards[gameState.CapturedPiece].SetBit(targetSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                }
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);
            }

            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating saved values
            Castle = gameState.Castle;
            EnPassant = gameState.EnPassant;
            UniqueIdentifier = gameState.ZobristKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeNullMove()
        {
            Side = (Side)Utils.OppositeSide(Side);
            var oldEnPassant = EnPassant;
            EnPassant = BoardSquare.noSquare;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)oldEnPassant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnMakeNullMove(MakeMoveGameState gameState)
        {
            Side = (Side)Utils.OppositeSide(Side);
            EnPassant = gameState.EnPassant;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)EnPassant);
        }

        /// <summary>
        /// False if any of the kings has been captured, or if the opponent king is in check.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly bool IsValid()
        {
            var kingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)];
            var kingSquare = kingBitBoard == default ? -1 : kingBitBoard.GetLS1BIndex();

            var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
            var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

            return kingSquare >= 0 && oppositeKingSquare >= 0
                && !MakeMoveAttacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

        /// <summary>
        /// Lightweight version of <see cref="IsValid"/>
        /// False if the opponent king is in check.
        /// This method is meant to be invoked only after <see cref="Position(Position, Move)"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool WasProduceByAValidMove()
        {
            var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
            var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

            return oppositeKingSquare >= 0 && !MakeMoveAttacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<Move> AllPossibleMoves(Move[]? movePool = null) => MakeMoveMoveGenerator.GenerateAllMoves(this, movePool);
    }

    public readonly struct MakeMoveGameState
    {
        public readonly int CapturedPiece;

        public readonly int Castle;

        public readonly BoardSquare EnPassant;

        // TODO: save full Zobrist key?

        public MakeMoveGameState(int capturedPiece, int castle, BoardSquare enpassant)
        {
            CapturedPiece = capturedPiece;
            Castle = castle;
            EnPassant = enpassant;
        }
    }

    public readonly struct MakeMoveGameStateWithZobristKey
    {
        public readonly int CapturedPiece;

        public readonly int Castle;

        public readonly BoardSquare EnPassant;

        public readonly long ZobristKey;

        public MakeMoveGameStateWithZobristKey(int capturedPiece, int castle, BoardSquare enpassant, long zobristKey)
        {
            CapturedPiece = capturedPiece;
            Castle = castle;
            EnPassant = enpassant;
            ZobristKey = zobristKey;
        }
    }

    #region ;(

    public static class MakeMoveZobristTable
    {
        private static readonly long[,] _table = Initialize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PieceHash(int boardSquare, int piece) => _table[boardSquare, piece];

        /// <summary>
        /// Uses <see cref="Piece.P"/> and squares <see cref="BoardSquare.a1"/>-<see cref="BoardSquare.h1"/>
        /// </summary>
        /// <param name="enPassantSquare"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EnPassantHash(int enPassantSquare)
        {
            if (enPassantSquare == (int)BoardSquare.noSquare)
            {
                return default;
            }

#if DEBUG
            if (!Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare))
            {
                throw new ArgumentException($"{Constants.Coordinates[enPassantSquare]} is not a valid en-passant square");
            }
#endif

            var file = enPassantSquare % 8;

            return _table[file, (int)Piece.P];
        }

        /// <summary>
        /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SideHash()
        {
            return _table[(int)BoardSquare.h8, (int)Piece.p];
        }

        /// <summary>
        /// Uses <see cref="Piece.p"/> and
        /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
        /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
        /// </summary>
        /// <param name="castle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CastleHash(int castle)
        {
            long combinedHash = 0;

            if ((castle & (int)CastlingRights.WK) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.a8, (int)Piece.p];        // a8
            }

            if ((castle & (int)CastlingRights.WQ) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.b8, (int)Piece.p];        // b8
            }

            if ((castle & (int)CastlingRights.BK) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.c8, (int)Piece.p];        // c8
            }

            if ((castle & (int)CastlingRights.BQ) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.d8, (int)Piece.p];        // d8
            }

            return combinedHash;
        }

        /// <summary>
        /// Calculates from scratch the hash of a position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PositionHash(MakeMovePosition position)
        {
            long positionHash = 0;

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
                    {
                        positionHash ^= PieceHash(squareIndex, pieceIndex);
                    }
                }
            }

            positionHash ^= EnPassantHash((int)position.EnPassant)
                ^ SideHash()
                ^ CastleHash(position.Castle);

            return positionHash;
        }

        /// <summary>
        /// Initializes Zobrist table (long[64, 12])
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long[,] Initialize()
        {
            var zobristTable = new long[64, 12];
            var randomInstance = new Random(int.MaxValue);

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
                }
            }

            return zobristTable;
        }
    }

    public static class MakeMoveMoveGenerator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int TRUE = 1;

        /// <summary>
        /// Indexed by <see cref="Piece"/>.
        /// Checks are not considered
        /// </summary>
        private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks = new Func<int, BitBoard, BitBoard>[]
        {
            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.White, origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            (int origin, BitBoard occupancy) => MakeMoveAttacks.BishopAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.RookAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.QueenAttacks(origin, occupancy),
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],

            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.Black, origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            (int origin, BitBoard occupancy) => MakeMoveAttacks.BishopAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.RookAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.QueenAttacks(origin, occupancy),
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],
        };

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Move> GenerateAllMoves(MakeMovePosition position, Move[]? movePool = null, bool capturesOnly = false)
        {
#if DEBUG
            if (position.Side == Side.Both)
            {
                return new List<Move>();
            }
#endif

            movePool ??= new Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GeneratePawnMoves(ref localIndex, movePool, position, offset, capturesOnly);
            GenerateCastlingMoves(ref localIndex, movePool, position, offset);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position, capturesOnly);

            return movePool.Take(localIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePawnMoves(ref int localIndex, Move[] movePool, MakeMovePosition position, int offset, bool capturesOnly = false)
        {
            int sourceSquare, targetSquare;

            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            while (bitboard != default)
            {
                sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                var sourceRank = (sourceSquare / 8) + 1;

#if DEBUG
                if (sourceRank == 1 || sourceRank == 8)
                {
                    _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                    continue;
                }
#endif

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var targetRank = (singlePushSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                    }
                    else if (!capturesOnly)
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    if (!capturesOnly)
                    {
                        var doublePushSquare = sourceSquare + (2 * pawnPush);
                        if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                            && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                        {
                            movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                        }
                    }
                }

                var attacks = MakeMoveAttacks.PawnAttacks[(int)position.Side, sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
                }

                // Captures
                var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
                while (attackedSquares != default)
                {
                    targetSquare = attackedSquares.GetLS1BIndex();
                    attackedSquares.ResetLS1B();

                    var targetRank = (targetSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                    }
                    else
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateCastlingMoves(ref int localIndex, Move[] movePool, MakeMovePosition position, int offset)
        {
            var piece = (int)Piece.K + offset;
            var oppositeSide = (Side)Utils.OppositeSide(position.Side);

            int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

            // Castles
            if (position.Castle != default)
            {
                if (position.Side == Side.White)
                {
                    bool ise1Attacked = MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                    if (((position.Castle & (int)CastlingRights.WK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                        && !ise1Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.WQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                        && !ise1Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                    }
                }
                else
                {
                    bool ise8Attacked = MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                    if (((position.Castle & (int)CastlingRights.BK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                        && !ise8Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !ise8Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                    }
                }
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        /// <param name="position"></param>
        /// <param name="capturesOnly"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePieceMoves(ref int localIndex, Move[] movePool, int piece, MakeMovePosition position, bool capturesOnly = false)
        {
            var bitboard = position.PieceBitBoards[piece];
            int sourceSquare, targetSquare;

            while (bitboard != default)
            {
                sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                    & ~position.OccupancyBitBoards[(int)position.Side];

                while (attacks != default)
                {
                    targetSquare = attacks.GetLS1BIndex();
                    attacks.ResetLS1B();

                    if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                    }
                    else if (!capturesOnly)
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                    }
                }
            }
        }
    }

    public static class MakeMoveAttacks
    {
        private static readonly BitBoard[] _bishopOccupancyMasks;
        private static readonly BitBoard[] _rookOccupancyMasks;

        /// <summary>
        /// [64 (Squares), 512 (Occupancies)]
        /// Use <see cref="BishopAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[,] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="RookAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[,] _rookAttacks;

        /// <summary>
        /// [2 (B|W), 64 (Squares)]
        /// </summary>
        public static BitBoard[,] PawnAttacks { get; }
        public static BitBoard[] KnightAttacks { get; }
        public static BitBoard[] KingAttacks { get; }

        static MakeMoveAttacks()
        {
            KingAttacks = AttackGenerator.InitializeKingAttacks();
            PawnAttacks = AttackGenerator.InitializePawnAttacks();
            KnightAttacks = AttackGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookAttacks();
        }

        /// <summary>
        /// Get Bishop attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy & _bishopOccupancyMasks[squareIndex];
            occ *= Constants.BishopMagicNumbers[squareIndex];
            occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

            return _bishopAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Rook attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy & _rookOccupancyMasks[squareIndex];
            occ *= Constants.RookMagicNumbers[squareIndex];
            occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

            return _rookAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Queen attacks assuming current board occupancy
        /// Use <see cref="QueenAttacks(BitBoard, BitBoard)"/> if rook and bishop attacks are already calculated
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard QueenAttacks(int squareIndex, BitBoard occupancy)
        {
            return QueenAttacks(
                RookAttacks(squareIndex, occupancy),
                BishopAttacks(squareIndex, occupancy));
        }

        /// <summary>
        /// Get Queen attacks having rook and bishop attacks pre-calculated
        /// </summary>
        /// <param name="rookAttacks"></param>
        /// <param name="bishopAttacks"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks)
        {
            return rookAttacks | bishopAttacks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquaredAttackedBySide(int squaredIndex, MakeMovePosition position, Side sideToMove) =>
            IsSquaredAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            Utils.Assert(sideToMove != Side.Both);

            var offset = Utils.PieceOffset(sideToMove);

            // I tried to order them from most to least likely
            return
                IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKing(squareIndex, offset, piecePosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            Utils.Assert(sideToMove != Side.Both);

            var offset = Utils.PieceOffset(sideToMove);

            // I tried to order them from most to least likely
            return
                IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
        {
            bishopAttacks = BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
        {
            rookAttacks = RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
        {
            var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
            return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
        }
    }

    #endregion
}

#pragma warning restore S101, S1854 // Types should be named in PascalCase
