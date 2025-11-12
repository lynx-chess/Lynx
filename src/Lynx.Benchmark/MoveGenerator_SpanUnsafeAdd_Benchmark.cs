/*
 *  BenchmarkDotNet v0.15.3, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method   | data | Mean         | Error      | StdDev     | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----- |-------------:|-----------:|-----------:|-------------:|------:|--------:|----------:|------------:|
 *  | Original | 1    |     18.19 us |   0.096 us |   0.085 us |     18.21 us |  1.00 |    0.01 |         - |          NA |
 *  | Improved | 1    |     19.36 us |   0.381 us |   0.439 us |     19.24 us |  1.06 |    0.02 |         - |          NA |
 *  |          |      |              |            |            |              |       |         |           |             |
 *  | Original | 10   |    189.15 us |   1.858 us |   1.647 us |    188.77 us |  1.00 |    0.01 |         - |          NA |
 *  | Improved | 10   |    191.29 us |   3.793 us |   6.935 us |    186.69 us |  1.01 |    0.04 |         - |          NA |
 *  |          |      |              |            |            |              |       |         |           |             |
 *  | Original | 100  |  1,823.25 us |  16.198 us |  14.359 us |  1,820.78 us |  1.00 |    0.01 |         - |          NA |
 *  | Improved | 100  |  1,878.62 us |  31.291 us |  29.270 us |  1,889.58 us |  1.03 |    0.02 |         - |          NA |
 *  |          |      |              |            |            |              |       |         |           |             |
 *  | Original | 1000 | 18,679.44 us | 327.838 us | 306.660 us | 18,529.66 us |  1.00 |    0.02 |         - |          NA |
 *  | Improved | 1000 | 18,124.94 us |  59.831 us |  46.712 us | 18,108.83 us |  0.97 |    0.02 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.3, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method   | data | Mean         | Error      | StdDev     | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----- |-------------:|-----------:|-----------:|------:|--------:|----------:|------------:|
 *  | Original | 1    |     18.35 us |   0.358 us |   0.490 us |  1.00 |    0.04 |         - |          NA |
 *  | Improved | 1    |     18.32 us |   0.360 us |   0.354 us |  1.00 |    0.03 |         - |          NA |
 *  |          |      |              |            |            |       |         |           |             |
 *  | Original | 10   |    177.41 us |   2.695 us |   2.521 us |  1.00 |    0.02 |         - |          NA |
 *  | Improved | 10   |    182.41 us |   3.543 us |   3.791 us |  1.03 |    0.03 |         - |          NA |
 *  |          |      |              |            |            |       |         |           |             |
 *  | Original | 100  |  1,747.34 us |  34.781 us |  34.160 us |  1.00 |    0.03 |         - |          NA |
 *  | Improved | 100  |  1,829.62 us |  30.650 us |  27.170 us |  1.05 |    0.02 |         - |          NA |
 *  |          |      |              |            |            |       |         |           |             |
 *  | Original | 1000 | 17,755.53 us | 268.955 us | 251.580 us |  1.00 |    0.02 |         - |          NA |
 *  | Improved | 1000 | 18,233.59 us | 343.455 us | 337.319 us |  1.03 |    0.02 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.3, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *
 *  | Method   | data | Mean         | Error      | StdDev       | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----- |-------------:|-----------:|-------------:|-------------:|------:|--------:|----------:|------------:|
 *  | Original | 1    |     20.64 us |   0.626 us |     1.807 us |     20.30 us |  1.01 |    0.12 |         - |          NA |
 *  | Improved | 1    |     20.81 us |   0.665 us |     1.960 us |     20.59 us |  1.02 |    0.13 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 10   |    210.44 us |   6.212 us |    18.023 us |    208.62 us |  1.01 |    0.12 |         - |          NA |
 *  | Improved | 10   |    202.67 us |   6.840 us |    19.735 us |    201.32 us |  0.97 |    0.13 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 100  |  1,948.95 us | 100.561 us |   291.746 us |  1,920.38 us |  1.02 |    0.22 |         - |          NA |
 *  | Improved | 100  |  1,753.19 us |  69.693 us |   204.398 us |  1,751.63 us |  0.92 |    0.18 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 1000 | 16,728.10 us | 648.577 us | 1,912.345 us | 16,037.98 us |  1.01 |    0.16 |         - |          NA |
 *  | Improved | 1000 | 16,709.66 us | 655.515 us | 1,932.800 us | 16,868.56 us |  1.01 |    0.16 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.3, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method   | data | Mean         | Error      | StdDev       | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----- |-------------:|-----------:|-------------:|-------------:|------:|--------:|----------:|------------:|
 *  | Original | 1    |     38.55 us |   0.756 us |     1.595 us |     38.11 us |  1.00 |    0.06 |         - |          NA |
 *  | Improved | 1    |     37.54 us |   0.751 us |     1.482 us |     37.00 us |  0.98 |    0.05 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 10   |    385.61 us |   7.475 us |    15.100 us |    379.05 us |  1.00 |    0.05 |         - |          NA |
 *  | Improved | 10   |    368.82 us |   6.458 us |     8.840 us |    368.16 us |  0.96 |    0.04 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 100  |  3,886.17 us |  75.556 us |    87.010 us |  3,872.52 us |  1.00 |    0.03 |         - |          NA |
 *  | Improved | 100  |  3,721.59 us |  73.274 us |   132.129 us |  3,685.33 us |  0.96 |    0.04 |         - |          NA |
 *  |          |      |              |            |              |              |       |         |           |             |
 *  | Original | 1000 | 38,360.79 us | 745.886 us |   697.702 us | 38,151.45 us |  1.00 |    0.02 |         - |          NA |
 *  | Improved | 1000 | 37,152.60 us | 732.014 us | 1,182.067 us | 37,176.18 us |  0.97 |    0.03 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Benchmark;

public class MoveGenerator_SpanUnsafeAdd_Benchmark : BaseBenchmark
{
    private readonly Position[] _positions;

    public static IEnumerable<int> Data => [1, 10, 100, 1_000];

    public MoveGenerator_SpanUnsafeAdd_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position(fen))];
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Original(int data)
    {
        var total = 0;

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        for (int i = 0; i < data; ++i)
        {
            foreach (var position in _positions)
            {
                attacks.Clear();
                attacksBySide.Clear();
                position.CalculateThreats(ref evaluationContext);

                var movePool = ArrayPool<Move>.Shared.Rent(Constants.MaxNumberOfPseudolegalMovesInAPosition);
                var moves = MoveGenerator_SpanUnsafeAdd_Original.GenerateAllMoves(position, ref evaluationContext, movePool);

                total += moves.Length;

                ArrayPool<Move>.Shared.Return(movePool);
            }
        }

        return total;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Improved(int data)
    {
        var total = 0;

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        for (int i = 0; i < data; ++i)
        {
            foreach (var position in _positions)
            {
                attacks.Clear();
                attacksBySide.Clear();
                position.CalculateThreats(ref evaluationContext);

                var movePool = ArrayPool<Move>.Shared.Rent(Constants.MaxNumberOfPseudolegalMovesInAPosition);
                var moves = MoveGenerator_SpanUnsafeAdd_Improved.GenerateAllMoves(position, ref evaluationContext, movePool);

                total += moves.Length;

                ArrayPool<Move>.Shared.Return(movePool);
            }
        }

        return total;
    }

    static class MoveGenerator_SpanUnsafeAdd_Original
    {
        /// <summary>
        /// Indexed by <see cref="Piece"/>.
        /// Checks are not considered
        /// </summary>
        public static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
        [
#pragma warning disable IDE0350 // Use implicitly typed lambda
            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],

        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],
#pragma warning restore IDE0350 // Use implicitly typed lambda
    ];

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        [Obsolete("dev and test only")]
        internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

            Span<BitBoard> attacks = stackalloc BitBoard[12];
            Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            return (capturesOnly
                ? GenerateAllCaptures(position, ref evaluationContext, moves)
                : GenerateAllMoves(position, ref evaluationContext, moves)).ToArray();
        }

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<Move> GenerateAllMoves(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            Debug.Assert(position.Side != Side.Both);

            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
            GenerateKingMoves(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

            return movePool[..localIndex];
        }

        /// <summary>
        /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<Move> GenerateAllCaptures(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            Debug.Assert(position.Side != Side.Both);

            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
            GenerateKingCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

            return movePool[..localIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, Position position, int offset)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    var targetRank = (singlePushSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                        movePool[localIndex] = knightPromo + 3;         // Q
                        movePool[localIndex + 1] = knightPromo + 2;     // R
                        movePool[localIndex + 2] = knightPromo;         // N
                        movePool[localIndex + 3] = knightPromo + 1;     // B

                        localIndex += 4;
                    }
                    else
                    {
                        movePool[localIndex++] = singlePawnPush;

                        // Double pawn push
                        // Inside of the single pawn push if because singlePush square cannot be occupied either
                        if ((sourceRank == 2)        // position.Side == Side.Black is always true, otherwise targetRank would be 1
                            || (sourceRank == 7))    // position.Side == Side.White is always true, otherwise targetRank would be 8
                        {
                            var doublePushSquare = singlePushSquare + pawnPush;

                            if (!occupancy.GetBit(doublePushSquare))
                            {
                                movePool[localIndex++] = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                            }
                        }
                    }
                }

                var attacks = pawnAttacks[sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
                }

                // Captures
                var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];

                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                        movePool[localIndex] = knightPromo + 3;         // Q
                        movePool[localIndex + 1] = knightPromo + 2;     // R
                        movePool[localIndex + 2] = knightPromo;         // N
                        movePool[localIndex + 3] = knightPromo + 1;     // B;

                        localIndex += 4;
                    }
                    else
                    {
                        movePool[localIndex++] = pawnCapture;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, Position position, int offset)
        {
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    var targetRank = (singlePushSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                        movePool[localIndex] = knightPromo + 3;         // Q
                        movePool[localIndex + 1] = knightPromo + 2;     // R
                        movePool[localIndex + 2] = knightPromo;         // N
                        movePool[localIndex + 3] = knightPromo + 1;     // B

                        localIndex += 4;
                    }
                }

                var attacks = pawnAttacks[sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
                }

                // Captures
                var attackedSquares = attacks & oppositeSidePieces;
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];

                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                        movePool[localIndex] = knightPromo + 3;         // Q
                        movePool[localIndex + 1] = knightPromo + 2;     // R
                        movePool[localIndex + 2] = knightPromo;         // N
                        movePool[localIndex + 3] = knightPromo + 1;     // B

                        localIndex += 4;
                    }
                    else
                    {
                        movePool[localIndex++] = pawnCapture;
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position, ref EvaluationContext evaluationContext)
        {
            // TODO: move to position?
            var castlingRights = position.Castle;

            if (castlingRights != default)
            {
                var occupancy = position.OccupancyBitBoards[(int)Side.Both];

                if (position.Side == Side.White)
                {
                    if ((castlingRights & (int)CastlingRights.WK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.WhiteShortCastle;
                    }

                    if ((castlingRights & (int)CastlingRights.WQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))

                    {
                        movePool[localIndex++] = position.WhiteLongCastle;
                    }
                }
                else
                {
                    if ((castlingRights & (int)CastlingRights.BK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.BlackShortCastle;
                    }

                    if ((castlingRights & (int)CastlingRights.BQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.BlackLongCastle;
                    }
                }
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            ulong squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

            var pieceAttacks = _pieceAttacks[piece];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & squaresNotOccupiedByUs;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);

                    Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
                }
            }
        }

        /// <summary>
        /// Generate King moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateKingMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
                & ~position.OccupancyBitBoards[(int)position.Side]
                & ~evaluationContext.AttacksBySide[Utils.OppositeSide(position.Side)];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen capture moves.
        /// Could also generate King captures, but <see cref="GenerateKingCaptures(ref int, Span{int}, int, Position)"/> is more efficient.
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];
            var oppositeSide = Utils.OppositeSide(position.Side);

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            var pieceAttacks = _pieceAttacks[piece];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & oppositeSidePieces;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];
                    movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
                }
            }
        }

        /// <summary>
        /// Generate King capture moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateKingCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var oppositeSide = Utils.OppositeSide(position.Side);

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & position.OccupancyBitBoards[oppositeSide]
                & ~evaluationContext.AttacksBySide[oppositeSide];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                var capturedPiece = position.Board[targetSquare];
                movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanGenerateAtLeastAValidMove(Position position, ref EvaluationContext evaluationContext)
        {
            Debug.Assert(position.Side != Side.Both);

            var offset = Utils.PieceOffset(position.Side);

            return IsAnyPawnMoveValid(position, offset)
                || IsAnyKingMoveValid((int)Piece.K + offset, position, ref evaluationContext)    // in?
                || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
                || IsAnyPieceMoveValid((int)Piece.B + offset, position)
                || IsAnyPieceMoveValid((int)Piece.N + offset, position)
                || IsAnyPieceMoveValid((int)Piece.R + offset, position)
                || IsAnyCastlingMoveValid(position, ref evaluationContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyPawnMoveValid(Position position, int offset)
        {
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    var targetRank = (singlePushSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        // If any of the promotions isn't valid, it means that the pawn move unveils a discovered check, or that the promoted piece doesn't stop an existing check in the 8th rank
                        // Therefore none of the other promotions will be valid either
                        if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q + offset)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (IsValidMove(position, singlePawnPush))
                        {
                            return true;
                        }

                        // Double pawn push
                        // Inside of the if because singlePush square cannot be occupied either
                        var doublePushSquare = singlePushSquare + pawnPush;
                        if (!occupancy.GetBit(doublePushSquare)
                            && (sourceRank == 2         // position.Side == Side.Black is always true, otherwise targetRank would be 1
                                || sourceRank == 7)     // position.Side == Side.White is always true, otherwise targetRank would be 8
                            && IsValidMove(position, MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece)))
                        {
                            return true;
                        }
                    }
                }

                var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant)
                    // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                    && IsValidMove(position, MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece))) // Could add here capturedPiece: (int)Piece.p - offset
                {
                    return true;
                }

                // Captures
                var attackedSquares = attacks & oppositeSidePieces;
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];

                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        // If any of the promotions that capture the same piece isn't valid, it means that the pawn move unveils a discovered check, or that the capture doesn't stop an existing check in the 8th rank
                        // Therefore none of the other promotions capturing the same piece will be valid either
                        if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q + offset)))
                        {
                            return true;
                        }
                    }
                    else if (IsValidMove(position, pawnCapture))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAnyCastlingMoveValid(Position position, ref EvaluationContext evaluationContext)
        {
            // TODO: move to position?

            var castlingRights = position.Castle;

            if (castlingRights != default)
            {
                var occupancy = position.OccupancyBitBoards[(int)Side.Both];

                if (position.Side == Side.White)
                {
                    if ((castlingRights & (int)CastlingRights.WK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                        && IsValidMove(position, position.WhiteShortCastle))
                    {
                        return true;
                    }

                    if ((castlingRights & (int)CastlingRights.WQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                        && IsValidMove(position, position.WhiteLongCastle))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((castlingRights & (int)CastlingRights.BK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                        && IsValidMove(position, position.BlackShortCastle))
                    {
                        return true;
                    }

                    if ((castlingRights & (int)CastlingRights.BQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                        && IsValidMove(position, position.BlackLongCastle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Also valid for Kings, but less performant than <see cref="IsAnyKingMoveValid(int, Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyPieceMoveValid(int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

            var pieceAttacks = _pieceAttacks[piece];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & squaresNotOccupiedByUs;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);

                    Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                    if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyKingMoveValid(int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
                & ~position.OccupancyBitBoards[(int)position.Side]
                & ~evaluationContext.AttacksBySide[Utils.OppositeSide(position.Side)];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValidMove(Position position, Move move)
        {
            var gameState = position.MakeMove(move);

            bool result = position.WasProduceByAValidMove();
            position.UnmakeMove(move, gameState);

            return result;
        }
    }

    static class MoveGenerator_SpanUnsafeAdd_Improved
    {
        /// <summary>
        /// Indexed by <see cref="Piece"/>.
        /// Checks are not considered
        /// </summary>
        public static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
        [
#pragma warning disable IDE0350 // Use implicitly typed lambda
            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],

        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin]
#pragma warning restore IDE0350 // Use implicitly typed lambda
        ];

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        [Obsolete("dev and test only")]
        internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

            Span<BitBoard> attacks = stackalloc BitBoard[12];
            Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            return (capturesOnly
                ? GenerateAllCaptures(position, ref evaluationContext, moves)
                : GenerateAllMoves(position, ref evaluationContext, moves)).ToArray();
        }

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<Move> GenerateAllMoves(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            Debug.Assert(position.Side != Side.Both);

            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
            GenerateKingMoves(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

            return movePool[..localIndex];
        }

        /// <summary>
        /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<Move> GenerateAllCaptures(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            Debug.Assert(position.Side != Side.Both);

            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
            GenerateKingCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

            return movePool[..localIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, Position position, int offset)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                    var targetRank = (singlePushSquare >> 3) + 1;

                    if (targetRank == 1 || targetRank == 8)
                    {
                        // Promotion
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);
                        Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;         // Q
                        Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;     // R
                        Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;         // N
                        Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;     // B
                        localIndex += 4;
                    }
                    else
                    {
                        Unsafe.Add(ref movePoolRef, localIndex++) = singlePawnPush;
                        // Double pawn push
                        // Inside of the single pawn push if because singlePush square cannot be occupied either
                        if ((sourceRank == 2)        // position.Side == Side.Black is always true, otherwise targetRank would be 1
                            || (sourceRank == 7))    // position.Side == Side.White is always true, otherwise targetRank would be 8
                        {
                            var doublePushSquare = singlePushSquare + pawnPush;

                            if (!occupancy.GetBit(doublePushSquare))
                            {
                                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                            }
                        }
                    }
                }

                var attacks = pawnAttacks[sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
                }

                // Captures
                var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];
                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)
                    {
                        // Capture with promotion
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                        Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;
                        Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;
                        Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;
                        Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;

                        localIndex += 4;
                    }
                    else
                    {
                        Unsafe.Add(ref movePoolRef, localIndex++) = pawnCapture;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, Position position, int offset)
        {
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    var targetRank = (singlePushSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)
                    {
                        // Promotion
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                        Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;         // Q
                        Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;     // R
                        Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;         // N
                        Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;     // B

                        localIndex += 4;
                    }
                }

                var attacks = pawnAttacks[sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
                }

                // Captures
                var attackedSquares = attacks & oppositeSidePieces;
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];

                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)
                    {
                        // Capture with promotion
                        var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                        Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;
                        Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;
                        Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;
                        Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;

                        localIndex += 4;
                    }
                    else
                    {
                        Unsafe.Add(ref movePoolRef, localIndex++) = pawnCapture;
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position, ref EvaluationContext evaluationContext)
        {
            // TODO: move to position?
            var castlingRights = position.Castle;

            if (castlingRights != default)
            {
                var occupancy = position.OccupancyBitBoards[(int)Side.Both];

                if (position.Side == Side.White)
                {
                    if ((castlingRights & (int)CastlingRights.WK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.WhiteShortCastle;
                    }

                    if ((castlingRights & (int)CastlingRights.WQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))

                    {
                        movePool[localIndex++] = position.WhiteLongCastle;
                    }
                }
                else
                {
                    if ((castlingRights & (int)CastlingRights.BK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.BlackShortCastle;
                    }

                    if ((castlingRights & (int)CastlingRights.BQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                    {
                        movePool[localIndex++] = position.BlackLongCastle;
                    }
                }
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            ulong squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

            var pieceAttacks = _pieceAttacks[piece];

            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & squaresNotOccupiedByUs;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);

                    Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
                }
            }
        }

        /// <summary>
        /// Generate King moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateKingMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
                & ~position.OccupancyBitBoards[(int)position.Side]
                & ~evaluationContext.AttacksBySide[Utils.OppositeSide(position.Side)];

            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen capture moves.
        /// Could also generate King captures, but <see cref="GenerateKingCaptures(ref int, Span{int}, int, Position)"/> is more efficient.
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];
            var oppositeSide = Utils.OppositeSide(position.Side);

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            var pieceAttacks = _pieceAttacks[piece];

            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & oppositeSidePieces;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
                }
            }
        }

        /// <summary>
        /// Generate King capture moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateKingCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var oppositeSide = Utils.OppositeSide(position.Side);

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & position.OccupancyBitBoards[oppositeSide]
                & ~evaluationContext.AttacksBySide[oppositeSide];
            ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                var capturedPiece = position.Board[targetSquare];
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanGenerateAtLeastAValidMove(Position position, ref EvaluationContext evaluationContext)
        {
            Debug.Assert(position.Side != Side.Both);

            var offset = Utils.PieceOffset(position.Side);

            return IsAnyPawnMoveValid(position, offset)
                || IsAnyKingMoveValid((int)Piece.K + offset, position, ref evaluationContext)    // in?
                || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
                || IsAnyPieceMoveValid((int)Piece.B + offset, position)
                || IsAnyPieceMoveValid((int)Piece.N + offset, position)
                || IsAnyPieceMoveValid((int)Piece.R + offset, position)
                || IsAnyCastlingMoveValid(position, ref evaluationContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyPawnMoveValid(Position position, int offset)
        {
            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var sourceRank = (sourceSquare >> 3) + 1;

                Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!occupancy.GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    var targetRank = (singlePushSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        // If any of the promotions isn't valid, it means that the pawn move unveils a discovered check, or that the promoted piece doesn't stop an existing check in the 8th rank
                        // Therefore none of the other promotions will be valid either
                        if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q + offset)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (IsValidMove(position, singlePawnPush))
                        {
                            return true;
                        }

                        // Double pawn push
                        // Inside of the if because singlePush square cannot be occupied either
                        var doublePushSquare = singlePushSquare + pawnPush;
                        if (!occupancy.GetBit(doublePushSquare)
                            && (sourceRank == 2         // position.Side == Side.Black is always true, otherwise targetRank would be 1
                                || sourceRank == 7)     // position.Side == Side.White is always true, otherwise targetRank would be 8
                            && IsValidMove(position, MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece)))
                        {
                            return true;
                        }
                    }
                }

                var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant)
                    // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                    && IsValidMove(position, MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece))) // Could add here capturedPiece: (int)Piece.p - offset
                {
                    return true;
                }

                // Captures
                var attackedSquares = attacks & oppositeSidePieces;
                while (attackedSquares != default)
                {
                    attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                    var capturedPiece = position.Board[targetSquare];

                    var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                    var targetRank = (targetSquare >> 3) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        // If any of the promotions that capture the same piece isn't valid, it means that the pawn move unveils a discovered check, or that the capture doesn't stop an existing check in the 8th rank
                        // Therefore none of the other promotions capturing the same piece will be valid either
                        if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q + offset)))
                        {
                            return true;
                        }
                    }
                    else if (IsValidMove(position, pawnCapture))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAnyCastlingMoveValid(Position position, ref EvaluationContext evaluationContext)
        {
            // TODO: move to position?

            var castlingRights = position.Castle;

            if (castlingRights != default)
            {
                var occupancy = position.OccupancyBitBoards[(int)Side.Both];

                if (position.Side == Side.White)
                {
                    if ((castlingRights & (int)CastlingRights.WK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                        && IsValidMove(position, position.WhiteShortCastle))
                    {
                        return true;
                    }

                    if ((castlingRights & (int)CastlingRights.WQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                        && IsValidMove(position, position.WhiteLongCastle))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((castlingRights & (int)CastlingRights.BK) != default
                        && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                        && IsValidMove(position, position.BlackShortCastle))
                    {
                        return true;
                    }

                    if ((castlingRights & (int)CastlingRights.BQ) != default
                        && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                        && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                        && IsValidMove(position, position.BlackLongCastle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Also valid for Kings, but less performant than <see cref="IsAnyKingMoveValid(int, Position)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyPieceMoveValid(int piece, Position position)
        {
            var bitboard = position.PieceBitBoards[piece];

            var occupancy = position.OccupancyBitBoards[(int)Side.Both];
            var squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

            var pieceAttacks = _pieceAttacks[piece];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out int sourceSquare);

                var attacks = pieceAttacks(sourceSquare, occupancy)
                    & squaresNotOccupiedByUs;

                while (attacks != default)
                {
                    attacks = attacks.WithoutLS1B(out int targetSquare);

                    Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                    if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAnyKingMoveValid(int piece, Position position, ref EvaluationContext evaluationContext)
        {
            var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
                & ~position.OccupancyBitBoards[(int)position.Side]
                & ~evaluationContext.AttacksBySide[Utils.OppositeSide(position.Side)];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out var targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValidMove(Position position, Move move)
        {
            var gameState = position.MakeMove(move);

            bool result = position.WasProduceByAValidMove();
            position.UnmakeMove(move, gameState);

            return result;
        }
    }
}
