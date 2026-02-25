/*
 *
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *
 *  | Method                  | Fen                  | Repetitions | Mean      | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |------------------------ |--------------------- |------------ |----------:|---------:|---------:|------:|----------:|------------:|
 *  | MakeMove_Original       | 8/p7/(...)w - - [32] | 400         |  87.59 us | 0.131 us | 0.123 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | 8/p7/(...)w - - [32] | 400         |  87.48 us | 0.119 us | 0.105 us |  1.00 |         - |          NA |
 *  |                         |                      |             |           |          |          |       |           |             |
 *  | MakeMove_Original       | r3k2r(...)- 0 1 [68] | 400         | 668.70 us | 0.982 us | 0.820 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | r3k2r(...)- 0 1 [68] | 400         | 653.67 us | 1.284 us | 1.138 us |  0.98 |         - |          NA |
 *  |                         |                      |             |           |          |          |       |           |             |
 *  | MakeMove_Original       | rnbqk(...)- 0 1 [56] | 400         | 257.62 us | 0.207 us | 0.172 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | rnbqk(...)- 0 1 [56] | 400         | 255.49 us | 0.327 us | 0.306 us |  0.99 |         - |          NA |
 *
 *
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32370/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *
 *  | Method                  | Fen                  | Repetitions | Mean      | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |------------------------ |--------------------- |------------ |----------:|---------:|---------:|------:|----------:|------------:|
 *  | MakeMove_Original       | 8/p7/(...)w - - [32] | 400         |  82.91 us | 0.141 us | 0.118 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | 8/p7/(...)w - - [32] | 400         |  80.05 us | 0.129 us | 0.108 us |  0.97 |         - |          NA |
 *  |                         |                      |             |           |          |          |       |           |             |
 *  | MakeMove_Original       | r3k2r(...)- 0 1 [68] | 400         | 598.51 us | 1.076 us | 0.954 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | r3k2r(...)- 0 1 [68] | 400         | 563.57 us | 0.662 us | 0.619 us |  0.94 |         - |          NA |
 *  |                         |                      |             |           |          |          |       |           |             |
 *  | MakeMove_Original       | rnbqk(...)- 0 1 [56] | 400         | 235.53 us | 0.275 us | 0.230 us |  1.00 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | rnbqk(...)- 0 1 [56] | 400         | 227.75 us | 0.271 us | 0.211 us |  0.97 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *
 *  | Method                  | Fen                  | Repetitions | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------------ |--------------------- |------------ |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | MakeMove_Original       | 8/p7/(...)w - - [32] | 400         |  58.13 us |  1.993 us |  5.687 us |  56.41 us |  1.01 |    0.13 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | 8/p7/(...)w - - [32] | 400         |  61.40 us |  2.450 us |  7.147 us |  59.48 us |  1.07 |    0.16 |         - |          NA |
 *  |                         |                      |             |           |           |           |           |       |         |           |             |
 *  | MakeMove_Original       | r3k2r(...)- 0 1 [68] | 400         | 444.33 us | 22.306 us | 64.715 us | 422.90 us |  1.02 |    0.20 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | r3k2r(...)- 0 1 [68] | 400         | 428.49 us | 14.876 us | 43.393 us | 426.60 us |  0.98 |    0.16 |         - |          NA |
 *  |                         |                      |             |           |           |           |           |       |         |           |             |
 *  | MakeMove_Original       | rnbqk(...)- 0 1 [56] | 400         | 196.64 us |  9.635 us | 28.257 us | 192.78 us |  1.02 |    0.21 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | rnbqk(...)- 0 1 [56] | 400         | 165.61 us |  5.085 us | 14.915 us | 163.44 us |  0.86 |    0.14 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *
 *  | Method                  | Fen                  | Repetitions | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------------ |--------------------- |------------ |----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | MakeMove_Original       | 8/p7/(...)w - - [32] | 400         |  89.87 us |  1.780 us |  4.053 us |  1.00 |    0.06 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | 8/p7/(...)w - - [32] | 400         | 103.99 us |  4.698 us | 13.174 us |  1.16 |    0.16 |         - |          NA |
 *  |                         |                      |             |           |           |           |       |         |           |             |
 *  | MakeMove_Original       | r3k2r(...)- 0 1 [68] | 400         | 807.97 us | 30.013 us | 87.072 us |  1.01 |    0.15 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | r3k2r(...)- 0 1 [68] | 400         | 773.47 us | 17.289 us | 48.480 us |  0.97 |    0.12 |         - |          NA |
 *  |                         |                      |             |           |           |           |       |         |           |             |
 *  | MakeMove_Original       | rnbqk(...)- 0 1 [56] | 400         | 311.20 us |  9.761 us | 28.629 us |  1.01 |    0.13 |         - |          NA |
 *  | MakeMove_UnsafeArrayRef | rnbqk(...)- 0 1 [56] | 400         | 262.69 us |  5.230 us |  7.829 us |  0.85 |    0.08 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Benchmark;

public class MakeUnmakeMove_ArrayIndexVsUnsafeArrayRef_Benchmark : BaseBenchmark
{
    [Params(
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        "8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -")]
    public string Fen { get; set; } = Constants.InitialPositionFEN;

    [Params(400)]
    public int Repetitions { get; set; }

    private Position _referencePosition = default!;
    private Move[] _moves = default!;

    private MakeMoveState _originalState = default!;
    private MakeMoveState _unsafeState = default!;

    [GlobalSetup]
    public void Setup()
    {
        _referencePosition = new Position(Fen);

        Span<Bitboard> contextBuffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(contextBuffer);

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        _moves = MoveGenerator.GenerateAllMoves(_referencePosition, ref evaluationContext, movePool).ToArray();

        _originalState = new MakeMoveState(_referencePosition);
        _unsafeState = new MakeMoveState(_referencePosition);
    }

    [GlobalCleanup]
    public void Cleanup() => _referencePosition.Dispose();

    [Benchmark(Baseline = true)]
    public ulong MakeMove_Original()
    {
        ulong checksum = 0;

        for (int i = 0; i < Repetitions; ++i)
        {
            foreach (var move in _moves)
            {
                var gameState = _originalState.MakeMove_Original(move);
                checksum ^= _originalState.UniqueIdentifier;
                _originalState.UnmakeMove(move, gameState);
            }
        }

        return checksum;
    }

    [Benchmark]
    public ulong MakeMove_UnsafeArrayRef()
    {
        ulong checksum = 0;

        for (int i = 0; i < Repetitions; ++i)
        {
            foreach (var move in _moves)
            {
                var gameState = _unsafeState.MakeMove_UnsafeArrayRef(move);
                checksum ^= _unsafeState.UniqueIdentifier;
                _unsafeState.UnmakeMove(move, gameState);
            }
        }

        return checksum;
    }

    private sealed class MakeMoveState
    {
        public ulong UniqueIdentifier => _uniqueIdentifier;

        private ulong _uniqueIdentifier;
        private ulong _kingPawnUniqueIdentifier;
        private readonly ulong[] _nonPawnHash = new ulong[2];
        private ulong _minorHash;
        private ulong _majorHash;

        private readonly Bitboard[] _pieceBitboards = new Bitboard[12];
        private readonly Bitboard[] _occupancyBitboards = new Bitboard[3];
        private readonly int[] _board = new int[64];

        private byte _castle;
        private readonly byte[] _castlingRightsUpdateConstants;

        private BoardSquare _enPassant;
        private Side _side;

        internal int IncrementalEvalAccumulator;
        internal int IncrementalPhaseAccumulator;
        internal bool IsIncrementalEval;

        public MakeMoveState(Position position)
        {
            Array.Copy(position.PieceBitboards, _pieceBitboards, 12);
            Array.Copy(position.OccupancyBitboards, _occupancyBitboards, 3);
            Array.Copy(position.Board, _board, 64);

            _side = position.Side;
            _enPassant = position.EnPassant;
            _castle = position.Castle;

            _castlingRightsUpdateConstants = Constants.CastlingRightsUpdateConstants.ToArray();

            _uniqueIdentifier = 0;
            _kingPawnUniqueIdentifier = 0;
            _minorHash = 0;
            _majorHash = 0;

            _nonPawnHash[(int)Side.White] = 0;
            _nonPawnHash[(int)Side.Black] = 0;

            IncrementalEvalAccumulator = 0;
            IncrementalPhaseAccumulator = 0;
            IsIncrementalEval = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameState MakeMove_Original(Move move)
        {
            var gameState = new MakeMoveGameState(
                _castle,
                _enPassant,
                _uniqueIdentifier,
                _kingPawnUniqueIdentifier,
                _minorHash,
                _majorHash,
                _nonPawnHash[(int)Side.White],
                _nonPawnHash[(int)Side.Black],
                IncrementalEvalAccumulator,
                IncrementalPhaseAccumulator,
                IsIncrementalEval);

            var oldSide = (int)_side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            int extraPhaseIfIncremental = 0;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
                extraPhaseIfIncremental = GamePhaseByPiece[promotedPiece];
            }

            _pieceBitboards[piece].PopBit(sourceSquare);
            _occupancyBitboards[oldSide].PopBit(sourceSquare);
            _board[sourceSquare] = (int)Piece.None;

            _pieceBitboards[newPiece].SetBit(targetSquare);
            _occupancyBitboards[oldSide].SetBit(targetSquare);
            _board[targetSquare] = newPiece;

            var sourcePieceHash = ZobristTable.PieceHash(sourceSquare, piece);
            var targetPieceHash = ZobristTable.PieceHash(targetSquare, newPiece);
            var fullPieceMovementHash = sourcePieceHash ^ targetPieceHash;

            _uniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ fullPieceMovementHash
                ^ ZobristTable.EnPassantHash((int)_enPassant)
                ^ ZobristTable.CastleHash(_castle);

            if (piece == (int)Piece.P || piece == (int)Piece.p)
            {
                _kingPawnUniqueIdentifier ^= sourcePieceHash;

                if (promotedPiece == default)
                {
                    _kingPawnUniqueIdentifier ^= targetPieceHash;
                }
                else
                {
                    _nonPawnHash[oldSide] ^= targetPieceHash;

                    if (Utils.IsMinorPiece(newPiece))
                    {
                        _minorHash ^= targetPieceHash;
                    }
                    else if (Utils.IsMajorPiece(newPiece))
                    {
                        _majorHash ^= targetPieceHash;
                    }
                }
            }
            else
            {
                _nonPawnHash[oldSide] ^= fullPieceMovementHash;

                if (piece == (int)Piece.K || piece == (int)Piece.k)
                {
                    IsIncrementalEval = false;
                    _kingPawnUniqueIdentifier ^= fullPieceMovementHash;
                }
                else if (Utils.IsMinorPiece(piece))
                {
                    _minorHash ^= fullPieceMovementHash;
                }
                else if (Utils.IsMajorPiece(piece))
                {
                    _majorHash ^= fullPieceMovementHash;
                }
            }

            _enPassant = BoardSquare.noSquare;

            if (IsIncrementalEval)
            {
                var whiteKing = _pieceBitboards[(int)Piece.K].GetLS1BIndex();
                var blackKing = _pieceBitboards[(int)Piece.k].GetLS1BIndex();
                var whiteBucket = PSQTBucketLayout[whiteKing];
                var blackBucket = PSQTBucketLayout[blackKing ^ 56];

                int sameSideBucket = whiteBucket;
                int oppositeSideBucket = blackBucket;
                if (_side == Side.Black)
                {
                    (sameSideBucket, oppositeSideBucket) = (oppositeSideBucket, sameSideBucket);
                }

                IncrementalEvalAccumulator -= PSQT(sameSideBucket, oppositeSideBucket, piece, sourceSquare);
                IncrementalEvalAccumulator += PSQT(sameSideBucket, oppositeSideBucket, newPiece, targetSquare);

                IncrementalPhaseAccumulator += extraPhaseIfIncremental;

                switch (move.SpecialMoveFlag())
                {
                    case SpecialMoveType.None:
                        {
                            var capturedPiece = move.CapturedPiece();
                            if (capturedPiece != (int)Piece.None)
                            {
                                var capturedSquare = targetSquare;

                                _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                                _occupancyBitboards[oppositeSide].PopBit(capturedSquare);

                                var capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                                _uniqueIdentifier ^= capturedPieceHash;

                                if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                                {
                                    _kingPawnUniqueIdentifier ^= capturedPieceHash;
                                }
                                else
                                {
                                    _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                    if (Utils.IsMinorPiece(capturedPiece))
                                    {
                                        _minorHash ^= capturedPieceHash;
                                    }
                                    else if (Utils.IsMajorPiece(capturedPiece))
                                    {
                                        _majorHash ^= capturedPieceHash;
                                    }
                                }

                                IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);
                                IncrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
                            }

                            break;
                        }
                    case SpecialMoveType.DoublePawnPush:
                        {
                            var pawnPush = +8 - (oldSide * 16);
                            var enPassantSquare = sourceSquare + pawnPush;

                            _enPassant = (BoardSquare)enPassantSquare;
                            _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                            break;
                        }
                    case SpecialMoveType.EnPassant:
                        {
                            var oppositePawnIndex = (int)Piece.p - offset;

                            var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                            var capturedPiece = oppositePawnIndex;

                            _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitboards[oppositeSide].PopBit(capturedSquare);
                            _board[capturedSquare] = (int)Piece.None;

                            var capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPawnHash;
                            _kingPawnUniqueIdentifier ^= capturedPawnHash;

                            IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

                            break;
                        }
                }
            }
            else
            {
                switch (move.SpecialMoveFlag())
                {
                    case SpecialMoveType.None:
                        {
                            var capturedPiece = move.CapturedPiece();

                            if (capturedPiece != (int)Piece.None)
                            {
                                var capturedSquare = targetSquare;

                                _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                                _occupancyBitboards[oppositeSide].PopBit(capturedSquare);

                                ulong capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                                _uniqueIdentifier ^= capturedPieceHash;

                                if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                                {
                                    _kingPawnUniqueIdentifier ^= capturedPieceHash;
                                }
                                else
                                {
                                    _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                    if (Utils.IsMinorPiece(capturedPiece))
                                    {
                                        _minorHash ^= capturedPieceHash;
                                    }
                                    else if (Utils.IsMajorPiece(capturedPiece))
                                    {
                                        _majorHash ^= capturedPieceHash;
                                    }
                                }
                            }

                            break;
                        }
                    case SpecialMoveType.DoublePawnPush:
                        {
                            var pawnPush = +8 - (oldSide * 16);
                            var enPassantSquare = sourceSquare + pawnPush;

                            _enPassant = (BoardSquare)enPassantSquare;
                            _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                            break;
                        }
                    case SpecialMoveType.ShortCastle:
                        {
                            var rookSourceSquare = Configuration.EngineSettings.IsChess960
                                ? targetSquare
                                : Utils.ShortCastleRookSourceSquare(oldSide);
                            var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                            var rookIndex = (int)Piece.R + offset;

                            _pieceBitboards[rookIndex].PopBit(rookSourceSquare);

                            var kingTargetSquare = Utils.KingShortCastleSquare(oldSide);

                            if (Configuration.EngineSettings.IsChess960)
                            {
                                _pieceBitboards[newPiece].PopBit(targetSquare);
                                _occupancyBitboards[oldSide].PopBit(targetSquare);
                                _board[targetSquare] = (int)Piece.None;
                                var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                                _pieceBitboards[newPiece].SetBit(kingTargetSquare);
                                _occupancyBitboards[oldSide].SetBit(kingTargetSquare);
                                _board[kingTargetSquare] = newPiece;
                                var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                                var hashFix = hashToRevert ^ hashToApply;

                                _uniqueIdentifier ^= hashFix;
                                _nonPawnHash[oldSide] ^= hashFix;
                                _kingPawnUniqueIdentifier ^= hashFix;
                            }

                            if (rookSourceSquare != kingTargetSquare)
                            {
                                _occupancyBitboards[oldSide].PopBit(rookSourceSquare);
                                _board[rookSourceSquare] = (int)Piece.None;
                            }

                            _pieceBitboards[rookIndex].SetBit(rookTargetSquare);
                            _occupancyBitboards[oldSide].SetBit(rookTargetSquare);
                            _board[rookTargetSquare] = rookIndex;

                            var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                            _uniqueIdentifier ^= hashChange;
                            _nonPawnHash[oldSide] ^= hashChange;
                            _majorHash ^= hashChange;

                            break;
                        }
                    case SpecialMoveType.LongCastle:
                        {
                            var rookSourceSquare = Configuration.EngineSettings.IsChess960
                                ? targetSquare
                                : Utils.LongCastleRookSourceSquare(oldSide);
                            var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                            var rookIndex = (int)Piece.R + offset;

                            _pieceBitboards[rookIndex].PopBit(rookSourceSquare);

                            var kingTargetSquare = Utils.KingLongCastleSquare(oldSide);

                            if (Configuration.EngineSettings.IsChess960)
                            {
                                _pieceBitboards[newPiece].PopBit(targetSquare);
                                _occupancyBitboards[oldSide].PopBit(targetSquare);
                                _board[targetSquare] = (int)Piece.None;
                                var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                                _pieceBitboards[newPiece].SetBit(kingTargetSquare);
                                _occupancyBitboards[oldSide].SetBit(kingTargetSquare);
                                _board[kingTargetSquare] = newPiece;
                                var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                                var hashFix = hashToRevert ^ hashToApply;

                                _uniqueIdentifier ^= hashFix;
                                _nonPawnHash[oldSide] ^= hashFix;
                                _kingPawnUniqueIdentifier ^= hashFix;
                            }

                            if (rookSourceSquare != kingTargetSquare)
                            {
                                _occupancyBitboards[oldSide].PopBit(rookSourceSquare);
                                _board[rookSourceSquare] = (int)Piece.None;
                            }

                            _pieceBitboards[rookIndex].SetBit(rookTargetSquare);
                            _occupancyBitboards[oldSide].SetBit(rookTargetSquare);
                            _board[rookTargetSquare] = rookIndex;

                            var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                            _uniqueIdentifier ^= hashChange;
                            _nonPawnHash[oldSide] ^= hashChange;
                            _majorHash ^= hashChange;

                            break;
                        }
                    case SpecialMoveType.EnPassant:
                        {
                            var oppositePawnIndex = (int)Piece.p - offset;

                            var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                            var capturedPiece = oppositePawnIndex;

                            _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitboards[oppositeSide].PopBit(capturedSquare);
                            _board[capturedSquare] = (int)Piece.None;

                            ulong capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPawnHash;
                            _kingPawnUniqueIdentifier ^= capturedPawnHash;

                            break;
                        }
                }
            }

            _side = (Side)oppositeSide;
            _occupancyBitboards[2] = _occupancyBitboards[1] | _occupancyBitboards[0];

            _castle &= _castlingRightsUpdateConstants[sourceSquare];
            _castle &= _castlingRightsUpdateConstants[targetSquare];

            _uniqueIdentifier ^= ZobristTable.CastleHash(_castle);

            return gameState;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameState MakeMove_UnsafeArrayRef(Move move)
        {
            ref var pieceBitboardsRef = ref MemoryMarshal.GetArrayDataReference(_pieceBitboards);
            ref var occupancyBitboardsRef = ref MemoryMarshal.GetArrayDataReference(_occupancyBitboards);
            ref var boardRef = ref MemoryMarshal.GetArrayDataReference(_board);

            var gameState = new MakeMoveGameState(
                _castle,
                _enPassant,
                _uniqueIdentifier,
                _kingPawnUniqueIdentifier,
                _minorHash,
                _majorHash,
                _nonPawnHash[(int)Side.White],
                _nonPawnHash[(int)Side.Black],
                IncrementalEvalAccumulator,
                IncrementalPhaseAccumulator,
                IsIncrementalEval);

            var oldSide = (int)_side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            int extraPhaseIfIncremental = 0;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
                extraPhaseIfIncremental = GamePhaseByPiece[promotedPiece];
            }

            Unsafe.Add(ref pieceBitboardsRef, piece).PopBit(sourceSquare);
            Unsafe.Add(ref occupancyBitboardsRef, oldSide).PopBit(sourceSquare);
            Unsafe.Add(ref boardRef, sourceSquare) = (int)Piece.None;

            Unsafe.Add(ref pieceBitboardsRef, newPiece).SetBit(targetSquare);
            Unsafe.Add(ref occupancyBitboardsRef, oldSide).SetBit(targetSquare);
            Unsafe.Add(ref boardRef, targetSquare) = newPiece;

            var sourcePieceHash = ZobristTable.PieceHash(sourceSquare, piece);
            var targetPieceHash = ZobristTable.PieceHash(targetSquare, newPiece);
            var fullPieceMovementHash = sourcePieceHash ^ targetPieceHash;

            _uniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ fullPieceMovementHash
                ^ ZobristTable.EnPassantHash((int)_enPassant)
                ^ ZobristTable.CastleHash(_castle);

            if (piece == (int)Piece.P || piece == (int)Piece.p)
            {
                _kingPawnUniqueIdentifier ^= sourcePieceHash;

                if (promotedPiece == default)
                {
                    _kingPawnUniqueIdentifier ^= targetPieceHash;
                }
                else
                {
                    _nonPawnHash[oldSide] ^= targetPieceHash;

                    if (Utils.IsMinorPiece(newPiece))
                    {
                        _minorHash ^= targetPieceHash;
                    }
                    else if (Utils.IsMajorPiece(newPiece))
                    {
                        _majorHash ^= targetPieceHash;
                    }
                }
            }
            else
            {
                _nonPawnHash[oldSide] ^= fullPieceMovementHash;

                if (piece == (int)Piece.K || piece == (int)Piece.k)
                {
                    IsIncrementalEval = false;
                    _kingPawnUniqueIdentifier ^= fullPieceMovementHash;
                }
                else if (Utils.IsMinorPiece(piece))
                {
                    _minorHash ^= fullPieceMovementHash;
                }
                else if (Utils.IsMajorPiece(piece))
                {
                    _majorHash ^= fullPieceMovementHash;
                }
            }

            _enPassant = BoardSquare.noSquare;

            if (IsIncrementalEval)
            {
                var whiteKing = Unsafe.Add(ref pieceBitboardsRef, (int)Piece.K).GetLS1BIndex();
                var blackKing = Unsafe.Add(ref pieceBitboardsRef, (int)Piece.k).GetLS1BIndex();
                var whiteBucket = PSQTBucketLayout[whiteKing];
                var blackBucket = PSQTBucketLayout[blackKing ^ 56];

                int sameSideBucket = whiteBucket;
                int oppositeSideBucket = blackBucket;
                if (_side == Side.Black)
                {
                    (sameSideBucket, oppositeSideBucket) = (oppositeSideBucket, sameSideBucket);
                }

                IncrementalEvalAccumulator -= PSQT(sameSideBucket, oppositeSideBucket, piece, sourceSquare);
                IncrementalEvalAccumulator += PSQT(sameSideBucket, oppositeSideBucket, newPiece, targetSquare);

                IncrementalPhaseAccumulator += extraPhaseIfIncremental;

                switch (move.SpecialMoveFlag())
                {
                    case SpecialMoveType.None:
                        {
                            var capturedPiece = move.CapturedPiece();
                            if (capturedPiece != (int)Piece.None)
                            {
                                var capturedSquare = targetSquare;

                                Unsafe.Add(ref pieceBitboardsRef, capturedPiece).PopBit(capturedSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oppositeSide).PopBit(capturedSquare);

                                var capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                                _uniqueIdentifier ^= capturedPieceHash;

                                if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                                {
                                    _kingPawnUniqueIdentifier ^= capturedPieceHash;
                                }
                                else
                                {
                                    _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                    if (Utils.IsMinorPiece(capturedPiece))
                                    {
                                        _minorHash ^= capturedPieceHash;
                                    }
                                    else if (Utils.IsMajorPiece(capturedPiece))
                                    {
                                        _majorHash ^= capturedPieceHash;
                                    }
                                }

                                IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);
                                IncrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
                            }

                            break;
                        }
                    case SpecialMoveType.DoublePawnPush:
                        {
                            var pawnPush = +8 - (oldSide * 16);
                            var enPassantSquare = sourceSquare + pawnPush;

                            _enPassant = (BoardSquare)enPassantSquare;
                            _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                            break;
                        }
                    case SpecialMoveType.EnPassant:
                        {
                            var oppositePawnIndex = (int)Piece.p - offset;

                            var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                            var capturedPiece = oppositePawnIndex;

                            Unsafe.Add(ref pieceBitboardsRef, capturedPiece).PopBit(capturedSquare);
                            Unsafe.Add(ref occupancyBitboardsRef, oppositeSide).PopBit(capturedSquare);
                            Unsafe.Add(ref boardRef, capturedSquare) = (int)Piece.None;

                            var capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPawnHash;
                            _kingPawnUniqueIdentifier ^= capturedPawnHash;

                            IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

                            break;
                        }
                }
            }
            else
            {
                switch (move.SpecialMoveFlag())
                {
                    case SpecialMoveType.None:
                        {
                            var capturedPiece = move.CapturedPiece();

                            if (capturedPiece != (int)Piece.None)
                            {
                                var capturedSquare = targetSquare;

                                Unsafe.Add(ref pieceBitboardsRef, capturedPiece).PopBit(capturedSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oppositeSide).PopBit(capturedSquare);

                                ulong capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                                _uniqueIdentifier ^= capturedPieceHash;

                                if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                                {
                                    _kingPawnUniqueIdentifier ^= capturedPieceHash;
                                }
                                else
                                {
                                    _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                    if (Utils.IsMinorPiece(capturedPiece))
                                    {
                                        _minorHash ^= capturedPieceHash;
                                    }
                                    else if (Utils.IsMajorPiece(capturedPiece))
                                    {
                                        _majorHash ^= capturedPieceHash;
                                    }
                                }
                            }

                            break;
                        }
                    case SpecialMoveType.DoublePawnPush:
                        {
                            var pawnPush = +8 - (oldSide * 16);
                            var enPassantSquare = sourceSquare + pawnPush;

                            _enPassant = (BoardSquare)enPassantSquare;
                            _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                            break;
                        }
                    case SpecialMoveType.ShortCastle:
                        {
                            var rookSourceSquare = Configuration.EngineSettings.IsChess960
                                ? targetSquare
                                : Utils.ShortCastleRookSourceSquare(oldSide);
                            var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                            var rookIndex = (int)Piece.R + offset;

                            Unsafe.Add(ref pieceBitboardsRef, rookIndex).PopBit(rookSourceSquare);

                            var kingTargetSquare = Utils.KingShortCastleSquare(oldSide);

                            if (Configuration.EngineSettings.IsChess960)
                            {
                                Unsafe.Add(ref pieceBitboardsRef, newPiece).PopBit(targetSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).PopBit(targetSquare);
                                Unsafe.Add(ref boardRef, targetSquare) = (int)Piece.None;
                                var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                                Unsafe.Add(ref pieceBitboardsRef, newPiece).SetBit(kingTargetSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).SetBit(kingTargetSquare);
                                Unsafe.Add(ref boardRef, kingTargetSquare) = newPiece;
                                var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                                var hashFix = hashToRevert ^ hashToApply;

                                _uniqueIdentifier ^= hashFix;
                                _nonPawnHash[oldSide] ^= hashFix;
                                _kingPawnUniqueIdentifier ^= hashFix;
                            }

                            if (rookSourceSquare != kingTargetSquare)
                            {
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).PopBit(rookSourceSquare);
                                Unsafe.Add(ref boardRef, rookSourceSquare) = (int)Piece.None;
                            }

                            Unsafe.Add(ref pieceBitboardsRef, rookIndex).SetBit(rookTargetSquare);
                            Unsafe.Add(ref occupancyBitboardsRef, oldSide).SetBit(rookTargetSquare);
                            Unsafe.Add(ref boardRef, rookTargetSquare) = rookIndex;

                            var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                            _uniqueIdentifier ^= hashChange;
                            _nonPawnHash[oldSide] ^= hashChange;
                            _majorHash ^= hashChange;

                            break;
                        }
                    case SpecialMoveType.LongCastle:
                        {
                            var rookSourceSquare = Configuration.EngineSettings.IsChess960
                                ? targetSquare
                                : Utils.LongCastleRookSourceSquare(oldSide);
                            var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                            var rookIndex = (int)Piece.R + offset;

                            Unsafe.Add(ref pieceBitboardsRef, rookIndex).PopBit(rookSourceSquare);

                            var kingTargetSquare = Utils.KingLongCastleSquare(oldSide);

                            if (Configuration.EngineSettings.IsChess960)
                            {
                                Unsafe.Add(ref pieceBitboardsRef, newPiece).PopBit(targetSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).PopBit(targetSquare);
                                Unsafe.Add(ref boardRef, targetSquare) = (int)Piece.None;
                                var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                                Unsafe.Add(ref pieceBitboardsRef, newPiece).SetBit(kingTargetSquare);
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).SetBit(kingTargetSquare);
                                Unsafe.Add(ref boardRef, kingTargetSquare) = newPiece;
                                var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                                var hashFix = hashToRevert ^ hashToApply;

                                _uniqueIdentifier ^= hashFix;
                                _nonPawnHash[oldSide] ^= hashFix;
                                _kingPawnUniqueIdentifier ^= hashFix;
                            }

                            if (rookSourceSquare != kingTargetSquare)
                            {
                                Unsafe.Add(ref occupancyBitboardsRef, oldSide).PopBit(rookSourceSquare);
                                Unsafe.Add(ref boardRef, rookSourceSquare) = (int)Piece.None;
                            }

                            Unsafe.Add(ref pieceBitboardsRef, rookIndex).SetBit(rookTargetSquare);
                            Unsafe.Add(ref occupancyBitboardsRef, oldSide).SetBit(rookTargetSquare);
                            Unsafe.Add(ref boardRef, rookTargetSquare) = rookIndex;

                            var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                            _uniqueIdentifier ^= hashChange;
                            _nonPawnHash[oldSide] ^= hashChange;
                            _majorHash ^= hashChange;

                            break;
                        }
                    case SpecialMoveType.EnPassant:
                        {
                            var oppositePawnIndex = (int)Piece.p - offset;

                            var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                            var capturedPiece = oppositePawnIndex;

                            Unsafe.Add(ref pieceBitboardsRef, capturedPiece).PopBit(capturedSquare);
                            Unsafe.Add(ref occupancyBitboardsRef, oppositeSide).PopBit(capturedSquare);
                            Unsafe.Add(ref boardRef, capturedSquare) = (int)Piece.None;

                            ulong capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPawnHash;
                            _kingPawnUniqueIdentifier ^= capturedPawnHash;

                            break;
                        }
                }
            }

            _side = (Side)oppositeSide;
            Unsafe.Add(ref occupancyBitboardsRef, 2) = Unsafe.Add(ref occupancyBitboardsRef, 1) | Unsafe.Add(ref occupancyBitboardsRef, 0);

            _castle &= _castlingRightsUpdateConstants[sourceSquare];
            _castle &= _castlingRightsUpdateConstants[targetSquare];

            _uniqueIdentifier ^= ZobristTable.CastleHash(_castle);

            return gameState;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove(Move move, MakeMoveGameState gameState)
        {
            var oppositeSide = (int)_side;
            var side = Utils.OppositeSide(oppositeSide);
            _side = (Side)side;
            var offset = Utils.PieceOffset(side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            _pieceBitboards[newPiece].PopBit(targetSquare);
            _occupancyBitboards[side].PopBit(targetSquare);
            _board[targetSquare] = (int)Piece.None;

            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.None:
                    {
                        var capturedPiece = move.CapturedPiece();

                        if (capturedPiece != (int)Piece.None)
                        {
                            _pieceBitboards[capturedPiece].SetBit(targetSquare);
                            _occupancyBitboards[oppositeSide].SetBit(targetSquare);
                            _board[targetSquare] = capturedPiece;
                        }

                        break;
                    }
                case SpecialMoveType.ShortCastle:
                    {
                        int rookSourceSquare;
                        if (Configuration.EngineSettings.IsChess960)
                        {
                            var kingTargetSquare = Utils.KingShortCastleSquare(side);

                            _pieceBitboards[newPiece].PopBit(kingTargetSquare);
                            _occupancyBitboards[side].PopBit(kingTargetSquare);
                            _board[kingTargetSquare] = (int)Piece.None;

                            rookSourceSquare = targetSquare;
                        }
                        else
                        {
                            rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                        }

                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitboards[rookIndex].PopBit(rookTargetSquare);

                        _occupancyBitboards[side].PopBit(rookTargetSquare);
                        _board[rookTargetSquare] = (int)Piece.None;

                        _pieceBitboards[rookIndex].SetBit(rookSourceSquare);
                        _occupancyBitboards[side].SetBit(rookSourceSquare);
                        _board[rookSourceSquare] = rookIndex;

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        int rookSourceSquare;
                        if (Configuration.EngineSettings.IsChess960)
                        {
                            var kingTargetSquare = Utils.KingLongCastleSquare(side);

                            _pieceBitboards[newPiece].PopBit(kingTargetSquare);
                            _occupancyBitboards[side].PopBit(kingTargetSquare);
                            _board[kingTargetSquare] = (int)Piece.None;

                            rookSourceSquare = targetSquare;
                        }
                        else
                        {
                            rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                        }

                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitboards[rookIndex].PopBit(rookTargetSquare);

                        _occupancyBitboards[side].PopBit(rookTargetSquare);
                        _board[rookTargetSquare] = (int)Piece.None;

                        _pieceBitboards[rookIndex].SetBit(rookSourceSquare);
                        _occupancyBitboards[side].SetBit(rookSourceSquare);
                        _board[rookSourceSquare] = rookIndex;

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;
                        var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];

                        _pieceBitboards[oppositePawnIndex].SetBit(capturedPawnSquare);
                        _occupancyBitboards[oppositeSide].SetBit(capturedPawnSquare);
                        _board[capturedPawnSquare] = oppositePawnIndex;

                        break;
                    }
            }

            _pieceBitboards[piece].SetBit(sourceSquare);
            _occupancyBitboards[side].SetBit(sourceSquare);
            _board[sourceSquare] = piece;

            _occupancyBitboards[2] = _occupancyBitboards[1] | _occupancyBitboards[0];

            _castle = gameState.Castle;
            _enPassant = gameState.EnPassant;

            _uniqueIdentifier = gameState.ZobristKey;
            _kingPawnUniqueIdentifier = gameState.KingPawnKey;
            _minorHash = gameState.MinorKey;
            _majorHash = gameState.MajorKey;
            _nonPawnHash[(int)Side.White] = gameState.NonPawnWhiteKey;
            _nonPawnHash[(int)Side.Black] = gameState.NonPawnBlackKey;

            IncrementalEvalAccumulator = gameState.IncrementalEvalAccumulator;
            IncrementalPhaseAccumulator = gameState.IncrementalPhaseAccumulator;
            IsIncrementalEval = gameState.IsIncrementalEval;
        }
    }

    private readonly struct MakeMoveGameState
    {
        public byte Castle { get; }
        public BoardSquare EnPassant { get; }

        public ulong ZobristKey { get; }
        public ulong KingPawnKey { get; }
        public ulong MinorKey { get; }
        public ulong MajorKey { get; }
        public ulong NonPawnWhiteKey { get; }
        public ulong NonPawnBlackKey { get; }

        public int IncrementalEvalAccumulator { get; }
        public int IncrementalPhaseAccumulator { get; }
        public bool IsIncrementalEval { get; }

        public MakeMoveGameState(
            byte castle,
            BoardSquare enPassant,
            ulong zobristKey,
            ulong kingPawnKey,
            ulong minorKey,
            ulong majorKey,
            ulong nonPawnWhiteKey,
            ulong nonPawnBlackKey,
            int incrementalEvalAccumulator,
            int incrementalPhaseAccumulator,
            bool isIncrementalEval)
        {
            Castle = castle;
            EnPassant = enPassant;

            ZobristKey = zobristKey;
            KingPawnKey = kingPawnKey;
            MinorKey = minorKey;
            MajorKey = majorKey;
            NonPawnWhiteKey = nonPawnWhiteKey;
            NonPawnBlackKey = nonPawnBlackKey;

            IncrementalEvalAccumulator = incrementalEvalAccumulator;
            IncrementalPhaseAccumulator = incrementalPhaseAccumulator;
            IsIncrementalEval = isIncrementalEval;
        }
    }
}
