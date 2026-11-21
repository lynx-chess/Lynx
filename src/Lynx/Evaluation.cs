using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public unsafe partial class Position
{
    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    public (int Score, int Phase) StaticEvaluation() => StaticEvaluation(0);

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    public (int Score, int Phase) StaticEvaluation(int movesWithoutCaptureOrPawnMove)
    {
        var kingPawnTable = new PawnTableElement[Constants.KingPawnHashSize];

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        return StaticEvaluation(movesWithoutCaptureOrPawnMove, kingPawnTable, ref evaluationContext);
    }

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, int Phase) StaticEvaluation(int movesWithoutCaptureOrPawnMove, PawnTableElement[] pawnEvalTable, ref EvaluationContext evaluationContext)
    {
        //var result = OnlineTablebaseProber.EvaluationSearch(this, movesWithoutCaptureOrPawnMove, cancellationToken);
        //Debug.Assert(result < CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");
        //Debug.Assert(result > -CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");

        //if (result != OnlineTablebaseProber.NoResult)
        //{
        //    return result;
        //}

        int packedScore = 0;
        int gamePhase = 0;

        var whitePawns = PieceBitboards[(int)Piece.P];
        var blackPawns = PieceBitboards[(int)Piece.p];

        Bitboard whitePawnAttacks = whitePawns.ShiftUpRightAndLeft();
        Bitboard blackPawnAttacks = blackPawns.ShiftDownRightAndLeft();

        evaluationContext.AttacksBySide[(int)Side.White] = evaluationContext.Attacks[(int)Piece.P] = whitePawnAttacks;
        evaluationContext.AttacksBySide[(int)Side.Black] = evaluationContext.Attacks[(int)Piece.p] = blackPawnAttacks;

        var whiteKing = PieceBitboards[(int)Piece.K].GetLS1BIndex();
        var blackKing = PieceBitboards[(int)Piece.k].GetLS1BIndex();

        var whiteBucket = PSQTBucketLayout[whiteKing];
        var blackBucket = PSQTBucketLayout[blackKing ^ 56];

        Debug.Assert((uint)blackKing < KingRing.Length);
        int whitePawnKingRingAttacks = (whitePawnAttacks & EvaluationTablesPinned.KingRing[blackKing]).CountBits();
        evaluationContext.IncreaseKingRingAttacks((int)Side.White, whitePawnKingRingAttacks);

        Debug.Assert((uint)whiteKing < KingRing.Length);
        int blackPawnKingRingAttacks = (blackPawnAttacks & EvaluationTablesPinned.KingRing[whiteKing]).CountBits();
        evaluationContext.IncreaseKingRingAttacks((int)Side.Black, blackPawnKingRingAttacks);

        if (IsIncrementalEval)
        {
            packedScore = IncrementalEvalAccumulator;
            gamePhase = IncrementalPhaseAccumulator;

            var kingPawnIndex = _kingPawnUniqueIdentifier & Constants.KingPawnHashMask;
            ref var entry = ref pawnEvalTable[kingPawnIndex];

            // pawnEvalTable hit: We can reuse cached eval for pawn additional evaluation + PieceProtectedByPawnBonus + KingShieldBonus
            if (entry.Key == _kingPawnUniqueIdentifier)
            {
                packedScore += entry.PackedScore;
            }
            // Not hit in pawnEvalTable table
            else
            {
                var pawnScore = PawnStructureEvaluation(whitePawns, blackPawns, whitePawnAttacks, blackPawnAttacks, whiteKing, blackKing, whitePawnKingRingAttacks, blackPawnKingRingAttacks);

                var whitePawnsCopy = whitePawns;
                while (whitePawnsCopy.NotEmpty())
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                var blackPawnsCopy = blackPawns;
                while (blackPawnsCopy.NotEmpty())
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            // White pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = PieceBitboards[pieceIndex];

                Debug.Assert((uint)pieceIndex < PieceProtectedByPawnBonus.Length);
                packedScore += EvaluationTablesPinned.PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard.NotEmpty())
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = PieceBitboards[pieceIndex];

                // Pieces protected by pawns bonus
                Debug.Assert((uint)(pieceIndex - 6) < PieceProtectedByPawnBonus.Length);
                packedScore -= EvaluationTablesPinned.PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard.NotEmpty())
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    packedScore -= AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, blackBucket, whiteBucket, pieceIndex, (int)Side.Black, whitePawnAttacks, whiteKing);
                }
            }
        }
        else
        {
            IncrementalEvalAccumulator = 0;
            IncrementalPhaseAccumulator = 0;

            var kingPawnIndex = _kingPawnUniqueIdentifier & Constants.KingPawnHashMask;
            ref var entry = ref pawnEvalTable[kingPawnIndex];

            // pawnTable hit: We can reuse cached eval for pawn structure + each individual pawn additional evaluation
            if (entry.Key == _kingPawnUniqueIdentifier)
            {
                packedScore += entry.PackedScore;

                // White pawns
                var whitePawnsCopy = PieceBitboards[(int)Piece.P];
                while (whitePawnsCopy.NotEmpty())
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, (int)Piece.P, pieceSquareIndex);
                }

                // Black pawns
                var blackPawnsCopy = PieceBitboards[(int)Piece.p];
                while (blackPawnsCopy.NotEmpty())
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, (int)Piece.p, pieceSquareIndex);
                }
            }
            // Not hit in pawnTable table
            else
            {
                var pawnScore = PawnStructureEvaluation(whitePawns, blackPawns, whitePawnAttacks, blackPawnAttacks, whiteKing, blackKing, whitePawnKingRingAttacks, blackPawnKingRingAttacks);

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = whitePawns;
                while (whitePawnsCopy.NotEmpty())
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, (int)Piece.P, pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                var blackPawnsCopy = blackPawns;
                while (blackPawnsCopy.NotEmpty())
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, (int)Piece.p, pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            ref var phaseByPiece = ref MemoryMarshal.GetReference(GamePhaseByPiece);

            // White pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = PieceBitboards[pieceIndex];

                Debug.Assert((uint)pieceIndex < PieceProtectedByPawnBonus.Length);
                packedScore += EvaluationTablesPinned.PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard.NotEmpty())
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, pieceIndex, pieceSquareIndex);

                    IncrementalPhaseAccumulator += Unsafe.Add(ref phaseByPiece, pieceIndex);

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = PieceBitboards[pieceIndex];

                // Pieces protected by pawns bonus
                Debug.Assert((uint)(pieceIndex - 6) < PieceProtectedByPawnBonus.Length);
                packedScore -= EvaluationTablesPinned.PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard.NotEmpty())
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, pieceIndex, pieceSquareIndex);

                    IncrementalPhaseAccumulator += Unsafe.Add(ref phaseByPiece, pieceIndex);

                    packedScore -= AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, blackBucket, whiteBucket, pieceIndex, (int)Side.Black, whitePawnAttacks, whiteKing);
                }
            }

            packedScore += IncrementalEvalAccumulator;
            gamePhase += IncrementalPhaseAccumulator;
            IsIncrementalEval = true;
        }

        // Kings - they can't be incremental due to the king buckets
        packedScore +=
            PSQT(whiteBucket, blackBucket, (int)Piece.K, whiteKing)
            + PSQT(blackBucket, whiteBucket, (int)Piece.k, blackKing);

        packedScore +=
            KingAdditionalEvaluation(whiteKing, whiteBucket, (int)Side.White, blackPawnAttacks)
            - KingAdditionalEvaluation(blackKing, blackBucket, (int)Side.Black, whitePawnAttacks);

        Debug.Assert((uint)whiteKing < BoardLength);
        var whiteKingAttacks = Attacks.KingAttacks[whiteKing];
        evaluationContext.Attacks[(int)Piece.K] |= whiteKingAttacks;
        evaluationContext.AttacksBySide[(int)Side.White] |= whiteKingAttacks;

        Debug.Assert((uint)blackKing < BoardLength);
        var blackKingAttacks = Attacks.KingAttacks[blackKing];
        evaluationContext.Attacks[(int)Piece.k] |= blackKingAttacks;
        evaluationContext.AttacksBySide[(int)Side.Black] |= blackKingAttacks;

        // King mobility
        var whiteKingAttacksCount =
            (whiteKingAttacks
                & (~(whitePawns | blackPawnAttacks)))
            .CountBits();

        var blackKingAttacksCount =
            (blackKingAttacks
                & (~(blackPawns | whitePawnAttacks)))
            .CountBits();

        Debug.Assert((uint)whiteKingAttacksCount < KingMobilityBonus.Length);
        Debug.Assert((uint)blackKingAttacksCount < KingMobilityBonus.Length);
        packedScore += EvaluationTablesPinned.KingMobilityBonus[whiteKingAttacksCount]
            - EvaluationTablesPinned.KingMobilityBonus[blackKingAttacksCount];

        AssertAttackPopulation(ref evaluationContext);

        // Total king rings attacks
        Debug.Assert((uint)Math.Min(13, evaluationContext.WhiteKingRingAttacks) < TotalKingRingAttacksBonus.Length);
        Debug.Assert((uint)Math.Min(13, evaluationContext.BlackKingRingAttacks) < TotalKingRingAttacksBonus.Length);
        packedScore +=
            EvaluationTablesPinned.TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.WhiteKingRingAttacks)]
            - EvaluationTablesPinned.TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.BlackKingRingAttacks)];

        // Bishop pair bonus
        if (PieceBitboards[(int)Piece.B].CountBits() >= 2)
        {
            packedScore += BishopPairBonus;
        }

        if (PieceBitboards[(int)Piece.b].CountBits() >= 2)
        {
            packedScore -= BishopPairBonus;
        }

        // Pieces attacked by pawns bonus
        packedScore += PieceAttackedByPawnPenalty
            * ((blackPawnAttacks & OccupancyBitboards[(int)Side.White] /* & (~whitePawns) */).CountBits()
                - (whitePawnAttacks & OccupancyBitboards[(int)Side.Black] /* & (~blackPawns) */).CountBits());

        // Threats
        packedScore += Threats(ref evaluationContext, side: Side.White, oppositeSide: (int)Side.Black)
            - Threats(ref evaluationContext, side: Side.Black, oppositeSide: (int)Side.White);

        // Checks
        packedScore += Checks(ref evaluationContext, (int)Side.White, (int)Side.Black)
            - Checks(ref evaluationContext, (int)Side.Black, (int)Side.White);

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        int endGamePhase = MaxPhase - gamePhase;

        var middleGameScore = Utils.UnpackMG(packedScore);
        var endGameScore = Utils.UnpackEG(packedScore);
        var eval = ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / MaxPhase;

        int totalPawnsCount = whitePawns.CountBits() + blackPawns.CountBits();

        // Few pieces endgames
        if (gamePhase <= 5)
        {
            // Pawnless endgames
            if (totalPawnsCount == 0)
            {
                switch (gamePhase)
                {
                    case 5:
                        {
                            // RB vs R, RN vs R - scale it down due to the chances of it being a draw
                            if (PieceBitboards[(int)Piece.R].CountBits() == 1 && PieceBitboards[(int)Piece.r].CountBits() == 1)
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 4:
                        {
                            // Rook vs 2 minors and R vs r should be a draw
                            if ((PieceBitboards[(int)Piece.R].NotEmpty() && (PieceBitboards[(int)Piece.B] | PieceBitboards[(int)Piece.N]).Empty())
                                || (PieceBitboards[(int)Piece.r].NotEmpty() && (PieceBitboards[(int)Piece.b] | PieceBitboards[(int)Piece.n]).Empty()))
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 3:
                        {
                            var winningSideOffset = Utils.PieceOffset(eval >= 0);

                            if (PieceBitboards[(int)Piece.N + winningSideOffset].CountBits() == 2)      // NN vs N, NN vs B
                            {
                                return (EvaluationConstants.DrawScore, gamePhase);
                            }

                            // Rook vs a minor is a draw
                            // Without rooks, only BB vs N is a win and BN vs N can have some chances

                            eval >>= 1; // /2

                            break;
                        }
                    case 2:
                        {
                            var whiteKnightsCount = PieceBitboards[(int)Piece.N].CountBits();

                            if (whiteKnightsCount + PieceBitboards[(int)Piece.n].CountBits() == 2            // NN vs -, N vs N
                                    || whiteKnightsCount + PieceBitboards[(int)Piece.B].CountBits() == 1)    // B vs N, B vs B
                            {
                                return (EvaluationConstants.DrawScore, gamePhase);
                            }

                            break;
                        }
                    case 1:
                    case 0:
                        {
                            return (EvaluationConstants.DrawScore, gamePhase);
                        }
                }
            }
            else
            {
                var winningSideOffset = Utils.PieceOffset(eval >= 0);

                if (gamePhase == 1)
                {
                    if (PieceBitboards[(int)Piece.B + winningSideOffset].NotEmpty()
                        && (PieceBitboards[(int)Piece.P + winningSideOffset] & Constants.NotAorH).Empty())
                    {
                        if (IsBishopPawnDraw(winningSideOffset))
                        {
                            return (EvaluationConstants.DrawScore, gamePhase);
                        }

                        // We can reduce the rest of positions, i.e. if the king hasn't reached the corner
                        // This also reduces won positions, but it shouldn't matter
                        eval >>= 1; // /2
                    }
                }
                else if (gamePhase == 2)
                {
                    if (totalPawnsCount == 1)
                    {
                        eval >>= 1; // /2
                    }

                    var whiteBishops = PieceBitboards[(int)Piece.B];
                    var blackBishops = PieceBitboards[(int)Piece.b];

                    // Opposite color bishop endgame with pawns are even more drawish
                    if (whiteBishops > 0
                        && blackBishops > 0
                        && BoardSquareExtensions.DifferentColor(
                            whiteBishops.GetLS1BIndex(),
                            blackBishops.GetLS1BIndex()))
                    {
                        eval >>= 1; // /2
                    }
                }
            }
        }

        // Endgame scaling with pawn count, formula yoinked from Sirius
        eval = (int)(eval * ((80 + (totalPawnsCount * 7)) / 128.0));

        // 50 moves rule distance scaling
        eval = ScaleEvalWith50MovesDrawDistance(eval, movesWithoutCaptureOrPawnMove);

        eval = Math.Clamp(eval, MinStaticEval, MaxStaticEval);

        var sideEval = _side == Side.White
            ? eval
            : -eval;

        return (sideEval, gamePhase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Phase()
    {
        var gamePhase = IsIncrementalEval
            ? IncrementalPhaseAccumulator
            : PhaseFromScratch();

        return (gamePhase > MaxPhase)    // Early promotions
            ? MaxPhase
            : gamePhase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int PhaseFromScratch()
    {
        ref var phaseByPiece = ref MemoryMarshal.GetReference(GamePhaseByPiece);

        return (Knights.CountBits() * Unsafe.Add(ref phaseByPiece, (int)Piece.N))
            + (Bishops.CountBits() * Unsafe.Add(ref phaseByPiece, (int)Piece.B))
            + (Rooks.CountBits() * Unsafe.Add(ref phaseByPiece, (int)Piece.R))
            + (Queens.CountBits() * Unsafe.Add(ref phaseByPiece, (int)Piece.Q));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TaperedEvaluation(int taperedEvaluationTerm, int phase)
    {
        return ((Utils.UnpackMG(taperedEvaluationTerm) * phase) + (Utils.UnpackEG(taperedEvaluationTerm) * (24 - phase))) / 24;
    }

    /// <summary>
    /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
    /// this method determines if a position is a result of either a loss by checkmate or a draw by stalemate.
    /// NegaMax style
    /// </summary>
    /// <param name="ply">Modulates the output, favouring positions with lower ply (i.e. Checkmate in less moves)</param>
    /// <returns>At least <see cref="CheckMateEvaluation"/> if Position.Side lost (more extreme values when <paramref name="ply"/> increases)
    /// or 0 if Position.Side was stalemated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int EvaluateFinalPosition(int ply, bool isInCheck)
    {
        if (isInCheck)
        {
            return MatedScore(ply);
        }

        return EvaluationConstants.DrawScore;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MatedScore(int ply) => -CheckMateBaseEvaluation + ply;

    /// <summary>
    /// Doesn't include <see cref="Piece.P"/>, <see cref="Piece.p"/>, <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AdditionalPieceEvaluation(ref EvaluationContext evaluationContext, int pieceSquareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, Bitboard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        return pieceIndex switch
        {
            (int)Piece.R or (int)Piece.r => RookAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, bucket, oppositeSideBucket, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.N or (int)Piece.n => KnightAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            _ => 0,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int PawnAdditionalEvaluation(ref EvaluationContext evaluationContext, int bucket, int oppositeSideBucket, int squareIndex, int pieceIndex, int sameSideKingSquare, int oppositeSideKingSquare)
    {
        int packedBonus = 0;

        var rank = Constants.Rank(squareIndex);
        var oppositeSide = (int)Side.Black;
        ulong passedPawnsMask;
        int pushSquare;

        if (pieceIndex == (int)Piece.p)
        {
            rank = 7 - rank;
            oppositeSide = (int)Side.White;
            Debug.Assert((uint)squareIndex < Masks.BlackPassedPawnMasks.Length);
            passedPawnsMask = EvaluationTablesPinned.BlackPassedPawnMasks[squareIndex];
            pushSquare = squareIndex + 8;
        }
        else
        {
            Debug.Assert((uint)squareIndex < Masks.WhitePassedPawnMasks.Length);
            passedPawnsMask = EvaluationTablesPinned.WhitePassedPawnMasks[squareIndex];
            pushSquare = squareIndex - 8;
        }

        var oppositeSidePawns = PieceBitboards[(int)Piece.p - pieceIndex];

        // Isolated pawn
        Debug.Assert((uint)squareIndex < Masks.IsolatedPawnMasks.Length);
        if ((PieceBitboards[pieceIndex] & EvaluationTablesPinned.IsolatedPawnMasks[squareIndex]).Empty())
        {
            Debug.Assert((uint)Constants.File(squareIndex) < IsolatedPawnPenalty.Length);
            packedBonus += EvaluationTablesPinned.IsolatedPawnPenalty[Constants.File(squareIndex)];
        }
        // Backwards pawn
        else if (!evaluationContext.Attacks[pieceIndex].GetBit(squareIndex)
            && (oppositeSidePawns.GetBit(pushSquare)                                            // Blocked
                || evaluationContext.Attacks[(int)Piece.p - pieceIndex].GetBit(pushSquare)))    // Push square attacked by opponent pawns
        {
            Debug.Assert((uint)rank < BackwardsPawnPenalty.Length);
            packedBonus += EvaluationTablesPinned.BackwardsPawnPenalty[rank];
        }

        // Passed pawn
        if ((oppositeSidePawns & passedPawnsMask).Empty())
        {
            // Passed pawn without opponent pieces ahead (in its passed pawn mask)
            if ((passedPawnsMask & OccupancyBitboards[oppositeSide]).Empty())
            {
                Debug.Assert((uint)bucket < PassedPawnNoEnemiesAheadBonus.Length && (uint)rank < PassedPawnNoEnemiesAheadBonus[bucket].Length);
                Debug.Assert((uint)oppositeSideBucket < PassedPawnNoEnemiesAheadEnemyBonus.Length && (uint)rank < PassedPawnNoEnemiesAheadEnemyBonus[oppositeSideBucket].Length);
                packedBonus += EvaluationTablesPinned.PassedPawnNoEnemiesAheadBonus[((bucket) * 8) + (rank)];
                packedBonus += EvaluationTablesPinned.PassedPawnNoEnemiesAheadEnemyBonus[((oppositeSideBucket) * 8) + (rank)];
            }

            // King distance to passed pawn
            Debug.Assert((uint)squareIndex < Constants.ChebyshevDistance.Length && (uint)sameSideKingSquare < Constants.ChebyshevDistance[squareIndex].Length);
            var friendlyKingDistance = EvaluationTablesPinned.ChebyshevDistance[((squareIndex) * 64) + (sameSideKingSquare)];

            // Enemy king distance to passed pawn
            Debug.Assert((uint)squareIndex < Constants.ChebyshevDistance.Length && (uint)oppositeSideKingSquare < Constants.ChebyshevDistance[squareIndex].Length);
            var enemyKingDistance = EvaluationTablesPinned.ChebyshevDistance[((squareIndex) * 64) + (oppositeSideKingSquare)];

            Debug.Assert((uint)bucket < PassedPawnBonus.Length && (uint)rank < PassedPawnBonus[bucket].Length);
            Debug.Assert((uint)oppositeSideBucket < PassedPawnEnemyBonus.Length && (uint)rank < PassedPawnEnemyBonus[oppositeSideBucket].Length);
            Debug.Assert((uint)friendlyKingDistance < FriendlyKingDistanceToPassedPawnBonus.Length);
            Debug.Assert((uint)enemyKingDistance < EnemyKingDistanceToPassedPawnPenalty.Length);
            packedBonus += EvaluationTablesPinned.PassedPawnBonus[((bucket) * 8) + (rank)]
                + EvaluationTablesPinned.PassedPawnEnemyBonus[((oppositeSideBucket) * 8) + (rank)]
                + EvaluationTablesPinned.FriendlyKingDistanceToPassedPawnBonus[friendlyKingDistance]
                + EvaluationTablesPinned.EnemyKingDistanceToPassedPawnPenalty[enemyKingDistance];
        }

        // Pawn phalanx
        if (Constants.File(squareIndex) != 7 && PieceBitboards[pieceIndex].GetBit(squareIndex + 1))
        {
            Debug.Assert((uint)rank < PawnPhalanxBonus.Length);
            packedBonus += EvaluationTablesPinned.PawnPhalanxBonus[rank];
        }

        return packedBonus;
    }

    /// <summary>
    /// Pawn structure evaluations terms that we save in pawnEvalTable, besides <see cref="PawnAdditionalEvaluation"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PawnStructureEvaluation(
        ulong whitePawns, ulong blackPawns,
        ulong whitePawnAttacks, ulong blackPawnAttacks,
        int whiteKing, int blackKing,
        int whitePawnKingRingAttacks, int blackPawnKingRingAttacks)
    {
        int pawnScore = 0;

        // King pawn shield bonus
        pawnScore += KingPawnShield(whiteKing, whitePawns, blackPawnAttacks);
        pawnScore -= KingPawnShield(blackKing, blackPawns, whitePawnAttacks);

        // Pieces protected by pawns bonus
        Debug.Assert((uint)Piece.P < PieceProtectedByPawnBonus.Length);
        pawnScore += EvaluationTablesPinned.PieceProtectedByPawnBonus[(int)Piece.P] * (whitePawnAttacks & whitePawns).CountBits();
        pawnScore -= EvaluationTablesPinned.PieceProtectedByPawnBonus[(int)Piece.P] * (blackPawnAttacks & blackPawns).CountBits();

        // King ring attacks
        pawnScore += PawnKingRingAttacksBonus * whitePawnKingRingAttacks;
        pawnScore -= PawnKingRingAttacksBonus * blackPawnKingRingAttacks;

        // Pawn islands
        pawnScore += PawnIslands(whitePawns, blackPawns);

        // Doubled pawns
        pawnScore += DoubledPawns(whitePawns);
        pawnScore -= DoubledPawns(blackPawns);

        return pawnScore;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int KnightAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, Bitboard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToKnightOffset = (int)Piece.N - (int)Piece.P;
        var sameSidePawns = PieceBitboards[pieceIndex - pawnToKnightOffset];

        Debug.Assert((uint)squareIndex < BoardLength);
        var attacks = Attacks.KnightAttacks[squareIndex];
        evaluationContext.Attacks[(int)Piece.N + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        Debug.Assert((uint)mobility < KnightMobilityBonus.Length);
        var packedBonus = EvaluationTablesPinned.KnightMobilityBonus[mobility];

        // King ring attacks
        Debug.Assert((uint)oppositeSideKingSquare < KingRing.Length);
        var kingRing = EvaluationTablesPinned.KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += KnightKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, Bitboard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToBishopOffset = (int)Piece.B - (int)Piece.P;
        var sameSidePawns = PieceBitboards[pieceIndex - pawnToBishopOffset];

        var offset = Utils.PieceOffset(pieceSide);

        var occupancy = OccupancyBitboards[(int)Side.Both];
        var attacks = Attacks.BishopAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.B + offset] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        Debug.Assert((uint)mobility < BishopMobilityBonus.Length);
        var packedBonus = EvaluationTablesPinned.BishopMobilityBonus[mobility];

        // King ring attacks
        Debug.Assert((uint)oppositeSideKingSquare < KingRing.Length);
        var kingRing = EvaluationTablesPinned.KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += BishopKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        // Bad bishop
        var sameColorPawns = sameSidePawns &
            (BoardSquareExtensions.IsDarkSquare(squareIndex)
                ? Constants.DarkSquaresBitboard
                : Constants.LightSquaresBitboard);

        // Allowing playing positions with > 8 pawns
        var sameColorPawnsCount = sameColorPawns.CountBits() % 9;

        Debug.Assert((uint)sameColorPawnsCount < BadBishop_SameColorPawnsPenalty.Length);
        packedBonus += EvaluationTablesPinned.BadBishop_SameColorPawnsPenalty[sameColorPawnsCount];

        // Blocked central pawns
        var sameSideCentralPawns = sameSidePawns & Constants.CentralFiles;

        var pawnBlockerSquares = pieceSide == (int)Side.White
            ? sameSideCentralPawns.ShiftUp()
            : sameSideCentralPawns.ShiftDown();

        var pawnBlockers = pawnBlockerSquares & OccupancyBitboards[Utils.OppositeSide(pieceSide)];

        Debug.Assert((uint)pawnBlockers.CountBits() < BadBishop_BlockedCentralPawnsPenalty.Length);
        packedBonus += EvaluationTablesPinned.BadBishop_BlockedCentralPawnsPenalty[pawnBlockers.CountBits()];

        // Bishop in unblocked long diagonals
        if ((attacks & Constants.CentralSquares).CountBits() == 2)
        {
            packedBonus += BishopInUnblockedLongDiagonalBonus;
        }

        if (!Configuration.EngineSettings.IsChess960        // Can't happen in standard chess
            || !Constants.Corners.GetBit(squareIndex))      // Saves some checks if no bishop in a corner at all
        {
            return packedBonus;
        }

        // Cornered/trapped bishop
        if (pieceIndex == (int)Piece.B)
        {
            if (squareIndex == (int)BoardSquare.a1 && Board[(int)BoardSquare.b2] == (int)Piece.P)
            {
                if (Board[(int)BoardSquare.b3] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
            else if (squareIndex == (int)BoardSquare.h1 && Board[(int)BoardSquare.g2] == (int)Piece.P)
            {
                if (Board[(int)BoardSquare.g3] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
        }
        else
        {
            if (squareIndex == (int)BoardSquare.a8 && Board[(int)BoardSquare.b7] == (int)Piece.p)
            {
                if (Board[(int)BoardSquare.b6] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
            else if (squareIndex == (int)BoardSquare.h8 && Board[(int)BoardSquare.g7] == (int)Piece.p)
            {
                if (Board[(int)BoardSquare.g6] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RookAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, Bitboard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;
        var sameSidePawns = PieceBitboards[pieceIndex - pawnToRookOffset];

        var occupancy = OccupancyBitboards[(int)Side.Both];
        var attacks = Attacks.RookAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.R + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        Debug.Assert((uint)mobility < RookMobilityBonus.Length);
        var packedBonus = EvaluationTablesPinned.RookMobilityBonus[mobility];

        // King ring attacks
        Debug.Assert((uint)oppositeSideKingSquare < KingRing.Length);
        var kingRing = EvaluationTablesPinned.KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += RookKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        var fileMask = Masks.FileMask(squareIndex);

        // Rook on open file
        if (((PieceBitboards[(int)Piece.P] | PieceBitboards[(int)Piece.p]) & fileMask).Empty())
        {
            var file = Constants.File(squareIndex);
            Debug.Assert((uint)bucket < OpenFileRookBonus.Length && (uint)file < OpenFileRookBonus[bucket].Length);
            Debug.Assert((uint)oppositeSideBucket < OpenFileRookEnemyBonus.Length && (uint)file < OpenFileRookEnemyBonus[oppositeSideBucket].Length);
            packedBonus += EvaluationTablesPinned.OpenFileRookBonus[((bucket) * 8) + (file)];
            packedBonus += EvaluationTablesPinned.OpenFileRookEnemyBonus[((oppositeSideBucket) * 8) + (file)];
        }
        // Rook on semi-open file
        else if ((sameSidePawns & fileMask).Empty())
        {
            var file = Constants.File(squareIndex);
            Debug.Assert((uint)bucket < SemiOpenFileRookBonus.Length && (uint)file < SemiOpenFileRookBonus[bucket].Length);
            Debug.Assert((uint)oppositeSideBucket < SemiOpenFileRookEnemyBonus.Length && (uint)file < SemiOpenFileRookEnemyBonus[oppositeSideBucket].Length);
            packedBonus += EvaluationTablesPinned.SemiOpenFileRookBonus[((bucket) * 8) + (file)];
            packedBonus += EvaluationTablesPinned.SemiOpenFileRookEnemyBonus[((oppositeSideBucket) * 8) + (file)];
        }

        // Connected rooks
        if ((attacks & PieceBitboards[pieceIndex]).CountBits() >= 1)
        {
            var rank = Constants.Rank(squareIndex);

            if (pieceIndex == (int)Piece.r)
            {
                rank = 7 - rank;
            }

            Debug.Assert((uint)rank < ConnectedRooksBonus.Length);
            packedBonus += EvaluationTablesPinned.ConnectedRooksBonus[rank];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int QueenAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, Bitboard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToQueenOffset = (int)Piece.Q - (int)Piece.P;
        var sameSidePawns = PieceBitboards[pieceIndex - pawnToQueenOffset];

        var occupancy = OccupancyBitboards[(int)Side.Both];
        var attacks = Attacks.QueenAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.Q + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        Debug.Assert((uint)mobility < QueenMobilityBonus.Length);
        var packedBonus = EvaluationTablesPinned.QueenMobilityBonus[mobility];

        // King ring attacks
        Debug.Assert((uint)oppositeSideKingSquare < KingRing.Length);
        var kingRing = EvaluationTablesPinned.KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += QueenKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int KingAdditionalEvaluation(int squareIndex, int bucket, int pieceSide, Bitboard enemyPawnAttacks)
    {
        // Virtual mobility (as if Queen)
        var attacksCount =
            (Attacks.QueenAttacks(squareIndex, OccupancyBitboards[(int)Side.Both])
            & ~(OccupancyBitboards[pieceSide] | enemyPawnAttacks)).CountBits();
        Debug.Assert((uint)attacksCount < VirtualKingMobilityBonus.Length);
        int packedBonus = EvaluationTablesPinned.VirtualKingMobilityBonus[attacksCount];

        var kingSideOffset = Utils.PieceOffset(pieceSide);

        // Opposite side rooks or queens on the board
        if ((PieceBitboards[(int)Piece.r - kingSideOffset] + PieceBitboards[(int)Piece.q - kingSideOffset]).NotEmpty())
        {
            var file = Masks.FileMask(squareIndex);

            // King on open file
            if (((PieceBitboards[(int)Piece.P] | PieceBitboards[(int)Piece.p]) & file).Empty())
            {
                Debug.Assert((uint)bucket < OpenFileKingPenalty.Length && (uint)Constants.File(squareIndex) < OpenFileKingPenalty[bucket].Length);
                packedBonus += EvaluationTablesPinned.OpenFileKingPenalty[((bucket) * 8) + (Constants.File(squareIndex))];
            }
            // King on semi-open file
            else if ((PieceBitboards[(int)Piece.P + kingSideOffset] & file).Empty())
            {
                Debug.Assert((uint)bucket < SemiOpenFileKingPenalty.Length && (uint)Constants.File(squareIndex) < SemiOpenFileKingPenalty[bucket].Length);
                packedBonus += EvaluationTablesPinned.SemiOpenFileKingPenalty[((bucket) * 8) + (Constants.File(squareIndex))];
            }
        }

        // Pawn king shield included next to pawn additional eval

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int KingPawnShield(int squareIndex, Bitboard sameSidePawns, Bitboard oppositeSidePawnAttacks)
    {
        Debug.Assert((uint)squareIndex < BoardLength);
        var kingShield = Attacks.KingAttacks[squareIndex] & sameSidePawns;
        var kingShieldCount = kingShield.CountBits();

        var nonAttackedShieldCount = (kingShield & (~oppositeSidePawnAttacks)).CountBits();

        return (KingShieldBonus * (kingShieldCount - nonAttackedShieldCount))
            + (KingShieldNonAttackedBonus * nonAttackedShieldCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DoubledPawns(Bitboard sameSidePawns)
    {
        var packedBonus = 0;

        var doubledPawns = sameSidePawns & sameSidePawns.ShiftUp();
        while (doubledPawns.NotEmpty())
        {
            doubledPawns = doubledPawns.WithoutLS1B(out var pieceSquareIndex);
            Debug.Assert((uint)Constants.File(pieceSquareIndex) < DoubledPawnPenalty.Length);
            packedBonus += EvaluationTablesPinned.DoubledPawnPenalty[Constants.File(pieceSquareIndex)];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PawnIslands(Bitboard whitePawns, Bitboard blackPawns)
    {
        var whiteIslandCount = CountPawnIslands(whitePawns);
        var blackIslandCount = CountPawnIslands(blackPawns);

        Debug.Assert((uint)whiteIslandCount < PawnIslandsBonus.Length);
        Debug.Assert((uint)blackIslandCount < PawnIslandsBonus.Length);
        return EvaluationTablesPinned.PawnIslandsBonus[whiteIslandCount] - EvaluationTablesPinned.PawnIslandsBonus[blackIslandCount];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountPawnIslands(Bitboard pawns)
        {
            byte pawnFileBitboard = 0;

            while (pawns.NotEmpty())
            {
                pawns = pawns.WithoutLS1B(out var squareIndex);

                // Bitboard.SetBit equivalent but for byte instead of ulong
                pawnFileBitboard |= (byte)(1 << (squareIndex % 8));
            }

            int shifted = pawnFileBitboard << 1;

            // Treat shifted’s MSB as 0 implicitly
            int starts = pawnFileBitboard & (~shifted);

            return BitOperations.PopCount((uint)starts);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateThreats(ref EvaluationContext evaluationContext)
    {
        var occupancy = OccupancyBitboards[(int)Side.Both];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = PieceBitboards[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board.NotEmpty())
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= MoveGenerator.PieceAttacks(pieceIndex, square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.White] |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = PieceBitboards[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board.NotEmpty())
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= MoveGenerator.PieceAttacks(pieceIndex, square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.Black] |= existingAttacks;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bitboard OppositeSideAttacks()
    {
        Bitboard oppositeSideAttacks = 0;

        var occupancy = OccupancyBitboards[(int)Side.Both];

        var oppositeSideOffset = Utils.PieceOffset(Utils.OppositeSide((int)_side));

        for (int pieceIndex = (int)Piece.P + oppositeSideOffset; pieceIndex <= (int)Piece.K + oppositeSideOffset; ++pieceIndex)
        {
            var board = PieceBitboards[pieceIndex];

            while (board.NotEmpty())
            {
                board = board.WithoutLS1B(out var square);
                oppositeSideAttacks |= MoveGenerator.PieceAttacks(pieceIndex, square, occupancy);
            }
        }

        return oppositeSideAttacks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Threats(ref EvaluationContext evaluationContext, Side side, int oppositeSide)
    {
        var occupancy = OccupancyBitboards[(int)Side.Both];
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = OccupancyBitboards[oppositeSide];
        var oppositeSidePawnIndex = (int)Piece.P + oppositeSideOffset;
        int packedBonus = 0;

        var attacks = evaluationContext.Attacks;
        var attacksBySide = evaluationContext.AttacksBySide;
        var board = Board;

        // Equivalent to just attacks[oppositeSidePawnIndex] due to how it's currently used,
        // since ~attacksBySide[(int)side] is always false given we're evaluation threats
        var defendedSquares = attacks[oppositeSidePawnIndex]
            | (attacksBySide[oppositeSide] & ~attacksBySide[(int)side]);

        for (int i = (int)Piece.N; i <= (int)Piece.K; ++i)
        {
            var ourPiecesIndex = 6 + i - oppositeSideOffset;
            var threats = attacks[ourPiecesIndex] & oppositeSidePieces;

            var defended = threats & defendedSquares;
            while (defended.NotEmpty())
            {
                defended = defended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                Debug.Assert((uint)i < EvaluationTablesPinned.DefendedThreatsBonusByPiece.Length && (uint)(attackedPiece - oppositeSideOffset) < EvaluationTablesPinned.DefendedThreatsBonusByPiece[i].Length);
                packedBonus += EvaluationTablesPinned.DefendedThreatsBonus[(i * 8) + (attackedPiece - oppositeSideOffset)];
            }

            var undefended = threats & ~defendedSquares;
            while (undefended.NotEmpty())
            {
                undefended = undefended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                Debug.Assert((uint)i < EvaluationTablesPinned.UndefendedThreatsBonusByPiece.Length && (uint)(attackedPiece - oppositeSideOffset) < EvaluationTablesPinned.UndefendedThreatsBonusByPiece[i].Length);
                packedBonus += EvaluationTablesPinned.UndefendedThreatsBonus[(i * 8) + (attackedPiece - oppositeSideOffset)];
            }
        }

        // Pawn push threats
        var ourPawns = PieceBitboards[(int)Piece.p - oppositeSidePawnIndex];
        var theirPawns = PieceBitboards[oppositeSidePawnIndex];

        var nonPawnEnemies = oppositeSidePieces & ~theirPawns;
        var safeSquaresToPush = ~defendedSquares;

        var pushes = ~occupancy & ourPawns.PawnPush(side);

        // Double pushes
        Debug.Assert((uint)(side == Side.White ? (int)BoardSquare.a3 : (int)BoardSquare.a6) < Masks.RankMasks.Length);
        var thirdRank = EvaluationTablesPinned.RankMasks[side == Side.White ? (int)BoardSquare.a3 : (int)BoardSquare.a6];
        var doublePushes = ~occupancy & (pushes & thirdRank).PawnPush(side);
        pushes |= doublePushes;

        var safePushes = pushes & safeSquaresToPush;
        var pushThreats = safePushes.PawnAttacks(side) & nonPawnEnemies;

        packedBonus += PawnPushThreatBonus * pushThreats.CountBits();

        // Passed pawn pushes
        while (safePushes.NotEmpty())
        {
            safePushes = safePushes.WithoutLS1B(out var safePush);
            var isWhite = side == Side.White;
            Debug.Assert((uint)safePush < (isWhite ? Masks.WhitePassedPawnMasks : Masks.BlackPassedPawnMasks).Length);
            var passedPawnMask = isWhite ? EvaluationTablesPinned.WhitePassedPawnMasks[safePush] : EvaluationTablesPinned.BlackPassedPawnMasks[safePush];

            if ((passedPawnMask & theirPawns).Empty())
            {
                var rank = isWhite ? Constants.Rank(safePush) : 7 - Constants.Rank(safePush);
                Debug.Assert((uint)rank < PassedPawnPushBonus.Length);
                packedBonus += EvaluationTablesPinned.PassedPawnPushBonus[rank];
            }
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Checks(ref EvaluationContext evaluationContext, int side, int oppositeSide)
    {
        int packedBonus = 0;

        var offset = Utils.PieceOffset(side);
        var occupancy = OccupancyBitboards[(int)Side.Both];

        var oppositeSideKingSquare = PieceBitboards[(int)Piece.k - offset].GetLS1BIndex();
        var oppositeSideAttacks = evaluationContext.AttacksBySide[oppositeSide];

        var bishopAttacks = Attacks.BishopAttacks(oppositeSideKingSquare, occupancy);
        var rookAttacks = Attacks.RookAttacks(oppositeSideKingSquare, occupancy);

        var attacks = evaluationContext.Attacks;

        Debug.Assert((uint)oppositeSideKingSquare < BoardLength);

        packedBonus += CheckBonus((int)Piece.N, attacks[(int)Piece.N + offset] & Attacks.KnightAttacks[oppositeSideKingSquare], oppositeSideAttacks);
        packedBonus += CheckBonus((int)Piece.B, attacks[(int)Piece.B + offset] & bishopAttacks, oppositeSideAttacks);
        packedBonus += CheckBonus((int)Piece.R, attacks[(int)Piece.R + offset] & rookAttacks, oppositeSideAttacks);
        packedBonus += CheckBonus((int)Piece.Q, attacks[(int)Piece.Q + offset] & Attacks.QueenAttacks(rookAttacks, bishopAttacks), oppositeSideAttacks);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CheckBonus(int piece, Bitboard checks, Bitboard oppositeSideAttacks)
        {
            var checksCount = checks.CountBits();

            var unsafeChecksCount = (checks & oppositeSideAttacks).CountBits();
            var safeChecksCount = checksCount - unsafeChecksCount;

            Debug.Assert((uint)piece < SafeCheckBonus.Length);
            Debug.Assert((uint)piece < UnsafeCheckBonus.Length);
            return (EvaluationTablesPinned.SafeCheckBonus[piece] * safeChecksCount) + (EvaluationTablesPinned.UnsafeCheckBonus[piece] * unsafeChecksCount);
        }

        return packedBonus;
    }

    /// <summary>
    /// Scales <paramref name="eval"/> with <paramref name="movesWithoutCaptureOrPawnMove"/>, so that
    /// an eval with 100 halfmove counter is half of the value of one with 0 halfmove counter
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ScaleEvalWith50MovesDrawDistance(int eval, int movesWithoutCaptureOrPawnMove) =>
        eval * (200 - movesWithoutCaptureOrPawnMove) / 200;

    /// <summary>
    /// If the pawn is in A or H files, the defending king reaches the corner/queening square or adjacent squares
    /// and the bishop is of the opposite color of the queening square, it's a draw.
    /// This method also takes into account the relative distance to the corner of both kings:
    /// if the defending one is closer enough, it's also a draw.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsBishopPawnDraw(int winningSideOffset)
    {
        var pawns = PieceBitboards[(int)Piece.P + winningSideOffset];

        bool hasAFilePawn = (pawns & Constants.AFile).NotEmpty();
        bool hasHFilePawn = (pawns & Constants.HFile).NotEmpty();

        // We filtered by Constants.NotAorH == 0 earlier, now we check that only one of those files has pawns
        if (hasAFilePawn == hasHFilePawn)
        {
            return false;
        }

        // 1 if black is winning
        var inverseWinningSide = winningSideOffset >> 2;

        const int whiteBlackDiff = (int)BoardSquare.a1 - (int)BoardSquare.a8;

        var promotionCornerSquare =
            (hasAFilePawn
                ? (int)BoardSquare.a8
                : (int)BoardSquare.h8)
            + (inverseWinningSide * whiteBlackDiff);

        var defendingKingSquare = PieceBitboards[(int)Piece.k - winningSideOffset].GetLS1BIndex();

        // Not in the corner or adjacent squares
        Debug.Assert((uint)defendingKingSquare < Constants.ChebyshevDistance.Length && (uint)promotionCornerSquare < Constants.ChebyshevDistance[defendingKingSquare].Length);
        if (EvaluationTablesPinned.ChebyshevDistance[((defendingKingSquare) * 64) + (promotionCornerSquare)] > 1)
        {
            return false;
        }

        var bishopSquare = PieceBitboards[(int)Piece.B + winningSideOffset].GetLS1BIndex();

        return BoardSquareExtensions.DifferentColor(bishopSquare, promotionCornerSquare);
    }
}
