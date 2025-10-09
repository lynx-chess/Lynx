using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public partial class Position
{
    private static readonly int[][] _defendedThreatsBonus =
    [
        [],
        KnightThreatsBonus_Defended,
        BishopThreatsBonus_Defended,
        RookThreatsBonus_Defended,
        QueenThreatsBonus_Defended,
        KingThreatsBonus_Defended
    ];

    private static readonly int[][] _undefendedThreatsBonus =
    [
        [],
        KnightThreatsBonus,
        BishopThreatsBonus,
        RookThreatsBonus,
        QueenThreatsBonus,
        KingThreatsBonus
    ];

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

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

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

        var whitePawns = _pieceBitBoards[(int)Piece.P];
        var blackPawns = _pieceBitBoards[(int)Piece.p];

        BitBoard whitePawnAttacks = whitePawns.ShiftUpRightAndLeft();
        BitBoard blackPawnAttacks = blackPawns.ShiftDownRightAndLeft();

        evaluationContext.AttacksBySide[(int)Side.White] = evaluationContext.Attacks[(int)Piece.P] = whitePawnAttacks;
        evaluationContext.AttacksBySide[(int)Side.Black] = evaluationContext.Attacks[(int)Piece.p] = blackPawnAttacks;

        var whiteKing = _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
        var blackKing = _pieceBitBoards[(int)Piece.k].GetLS1BIndex();

        var whiteBucket = PSQTBucketLayout[whiteKing];
        var blackBucket = PSQTBucketLayout[blackKing ^ 56];

        int whitePawnKingRingAttacks = (whitePawnAttacks & KingRing[blackKing]).CountBits();
        evaluationContext.IncreaseKingRingAttacks((int)Side.White, whitePawnKingRingAttacks);

        int blackPawnKingRingAttacks = (blackPawnAttacks & KingRing[whiteKing]).CountBits();
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
                var pawnScore = 0;

                // White pawns

                // King pawn shield bonus
                pawnScore += KingPawnShield(whiteKing, whitePawns, blackPawnAttacks);

                // Pieces protected by pawns bonus
                pawnScore += PieceProtectedByPawnBonus[(int)Piece.P] * (whitePawnAttacks & whitePawns).CountBits();

                // King ring attacks
                pawnScore += PawnKingRingAttacksBonus * whitePawnKingRingAttacks;

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = whitePawns;
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                // Black pawns

                // King pawn shield bonus
                pawnScore -= KingPawnShield(blackKing, blackPawns, whitePawnAttacks);

                // Pieces protected by pawns bonus
                pawnScore -= PieceProtectedByPawnBonus[(int)Piece.P] * (blackPawnAttacks & blackPawns).CountBits();

                // King ring attacks;
                pawnScore -= PawnKingRingAttacksBonus * blackPawnKingRingAttacks;

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = blackPawns;
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                // Pawn islands
                pawnScore += PawnIslands(whitePawns, blackPawns);

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            // White pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                packedScore += PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                // Pieces protected by pawns bonus
                packedScore -= PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard != default)
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

            // pawnTable hit: We can reuse cached eval for pawn additional evaluation + PieceProtectedByPawnBonus + KingShieldBonus
            if (entry.Key == _kingPawnUniqueIdentifier)
            {
                packedScore += entry.PackedScore;

                // White pawns
                // No PieceProtectedByPawnBonus - included in pawn table | packedScore += PieceProtectedByPawnBonus[...]

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = _pieceBitBoards[(int)Piece.P];
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, (int)Piece.P, pieceSquareIndex);

                    // No incremental eval - included in pawn table | packedScore += AdditionalPieceEvaluation(...);
                }

                // Black pawns
                // No PieceProtectedByPawnBonus - included in pawn table | packedScore -= PieceProtectedByPawnBonus .Length[...]

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = _pieceBitBoards[(int)Piece.p];
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, (int)Piece.p, pieceSquareIndex);

                    // No incremental eval - included in pawn table | packedScore -= AdditionalPieceEvaluation(...);
                }
            }
            // Not hit in pawnTable table
            else
            {
                var pawnScore = 0;

                // White pawns

                // King pawn shield bonus
                pawnScore += KingPawnShield(whiteKing, whitePawns, blackPawnAttacks);

                // Pieces protected by pawns bonus
                pawnScore += PieceProtectedByPawnBonus[(int)Piece.P] * (whitePawnAttacks & whitePawns).CountBits();

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = _pieceBitBoards[(int)Piece.P];
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, (int)Piece.P, pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                // Black pawns

                // King pawn shield bonus
                pawnScore -= KingPawnShield(blackKing, blackPawns, whitePawnAttacks);

                // Pieces protected by pawns bonus
                pawnScore -= PieceProtectedByPawnBonus[(int)Piece.P] * (blackPawnAttacks & blackPawns).CountBits();

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = _pieceBitBoards[(int)Piece.p];
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, (int)Piece.p, pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                // Pawn islands
                pawnScore += PawnIslands(whitePawns, blackPawns);

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            // White pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                packedScore += PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(whiteBucket, blackBucket, pieceIndex, pieceSquareIndex);

                    IncrementalPhaseAccumulator += GamePhaseByPiece[pieceIndex];

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                // Pieces protected by pawns bonus
                packedScore -= PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(blackBucket, whiteBucket, pieceIndex, pieceSquareIndex);

                    IncrementalPhaseAccumulator += GamePhaseByPiece[pieceIndex];

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

        var whiteKingAttacks = Attacks.KingAttacks[whiteKing];
        evaluationContext.Attacks[(int)Piece.K] |= whiteKingAttacks;
        evaluationContext.AttacksBySide[(int)Side.White] |= whiteKingAttacks;

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

        packedScore += KingMobilityBonus[whiteKingAttacksCount]
        - KingMobilityBonus[blackKingAttacksCount];

        AssertAttackPopulation(ref evaluationContext);

        // Total king rings ttacks
        packedScore +=
            TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.WhiteKingRingAttacks)]
            - TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.BlackKingRingAttacks)];

        // Bishop pair bonus
        if (_pieceBitBoards[(int)Piece.B].CountBits() >= 2)
        {
            packedScore += BishopPairBonus;
        }

        if (_pieceBitBoards[(int)Piece.b].CountBits() >= 2)
        {
            packedScore -= BishopPairBonus;
        }

        // Pieces attacked by pawns bonus
        packedScore += PieceAttackedByPawnPenalty
            * ((blackPawnAttacks & _occupancyBitBoards[(int)Side.White] /* & (~whitePawns) */).CountBits()
                - (whitePawnAttacks & _occupancyBitBoards[(int)Side.Black] /* & (~blackPawns) */).CountBits());

        // Threats
        packedScore += Threats(evaluationContext, oppositeSide: (int)Side.Black)
            - Threats(evaluationContext, oppositeSide: (int)Side.White);

        // Checks
        packedScore += Checks(evaluationContext, (int)Side.White, (int)Side.Black)
            - Checks(evaluationContext, (int)Side.Black, (int)Side.White);

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
                            if (_pieceBitBoards[(int)Piece.R].CountBits() == 1 && _pieceBitBoards[(int)Piece.r].CountBits() == 1)
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 4:
                        {
                            // Rook vs 2 minors and R vs r should be a draw
                            if ((_pieceBitBoards[(int)Piece.R] != 0 && (_pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.N]) == 0)
                                || (_pieceBitBoards[(int)Piece.r] != 0 && (_pieceBitBoards[(int)Piece.b] | _pieceBitBoards[(int)Piece.n]) == 0))
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 3:
                        {
                            var winningSideOffset = Utils.PieceOffset(eval >= 0);

                            if (_pieceBitBoards[(int)Piece.N + winningSideOffset].CountBits() == 2)      // NN vs N, NN vs B
                            {
                                return (0, gamePhase);
                            }

                            // Rook vs a minor is a draw
                            // Without rooks, only BB vs N is a win and BN vs N can have some chances

                            eval >>= 1; // /2

                            break;
                        }
                    case 2:
                        {
                            var whiteKnightsCount = _pieceBitBoards[(int)Piece.N].CountBits();

                            if (whiteKnightsCount + _pieceBitBoards[(int)Piece.n].CountBits() == 2            // NN vs -, N vs N
                                    || whiteKnightsCount + _pieceBitBoards[(int)Piece.B].CountBits() == 1)    // B vs N, B vs B
                            {
                                return (0, gamePhase);
                            }

                            break;
                        }
                    case 1:
                    case 0:
                        {
                            return (0, gamePhase);
                        }
                }
            }
            else
            {
                var winningSideOffset = Utils.PieceOffset(eval >= 0);
                var winningSidePawns = _pieceBitBoards[(int)Piece.P + winningSideOffset];

                if (gamePhase == 0)
                {
                    if (IsRookPawnDraw(winningSidePawns, winningSideOffset))
                    {
                        return (0, gamePhase);
                    }
                }
                if (gamePhase == 1)
                {
                    if (_pieceBitBoards[(int)Piece.B + winningSideOffset] != 0
                        && (winningSidePawns & Constants.NotAorH) == 0)
                    {
                        if (IsBishopPawnDraw(winningSidePawns, winningSideOffset))
                        {
                            return (0, gamePhase);
                        }

                        // We can reduce the rest of positions, i.e. if the king hasn't reached the corner
                        // This also reduces won positions, but it shouldn't matter
                        eval >>= 1; // /2
                    }
                }
                else if (gamePhase == 2)
                {
                    var whiteBishops = _pieceBitBoards[(int)Piece.B];
                    var blackBishops = _pieceBitBoards[(int)Piece.b];

                    // Opposite color bishop endgame with pawns are drawish
                    if (whiteBishops > 0
                        && blackBishops > 0
                        && Constants.DarkSquares[whiteBishops.GetLS1BIndex()] !=
                            Constants.DarkSquares[blackBishops.GetLS1BIndex()])
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
    private int PhaseFromScratch()
    {
        return (Knights.CountBits() * GamePhaseByPiece[(int)Piece.N])
            + (Bishops.CountBits() * GamePhaseByPiece[(int)Piece.B])
            + (Rooks.CountBits() * GamePhaseByPiece[(int)Piece.R])
            + (Queens.CountBits() * GamePhaseByPiece[(int)Piece.Q]);
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
            // Checkmate evaluation, but not as bad/shallow as it looks like since we're already searching at a certain depth
            return -CheckMateBaseEvaluation + ply;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Doesn't include <see cref="Piece.P"/>, <see cref="Piece.p"/>, <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AdditionalPieceEvaluation(ref EvaluationContext evaluationContext, int pieceSquareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        return pieceIndex switch
        {
            (int)Piece.R or (int)Piece.r => RookAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, bucket, oppositeSideBucket, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.N or (int)Piece.n => KnightAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            _ => 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int PawnAdditionalEvaluation(ref EvaluationContext evaluationContext, int bucket, int oppositeSideBucket, int squareIndex, int pieceIndex, int sameSideKingSquare, int oppositeSideKingSquare)
    {
        int packedBonus = 0;

        var rank = Constants.Rank[squareIndex];
        var oppositeSide = (int)Side.Black;
        ulong passedPawnsMask;
        int pushSquare;

        if (pieceIndex == (int)Piece.p)
        {
            rank = 7 - rank;
            oppositeSide = (int)Side.White;
            passedPawnsMask = Masks.BlackPassedPawnMasks[squareIndex];
            pushSquare = squareIndex + 8;
        }
        else
        {
            passedPawnsMask = Masks.WhitePassedPawnMasks[squareIndex];
            pushSquare = squareIndex - 8;
        }

        var oppositeSidePawns = _pieceBitBoards[(int)Piece.p - pieceIndex];

        // Isolated pawn
        if ((_pieceBitBoards[pieceIndex] & Masks.IsolatedPawnMasks[squareIndex]) == default)
        {
            packedBonus += IsolatedPawnPenalty[Constants.File[squareIndex]];
        }
        // Backwards pawn
        else if (!evaluationContext.Attacks[pieceIndex].GetBit(squareIndex)
            && (oppositeSidePawns.GetBit(pushSquare)                                            // Blocked
                || evaluationContext.Attacks[(int)Piece.p - pieceIndex].GetBit(pushSquare)))    // Push square attacked by opponent pawns
        {
            packedBonus += BackwardsPawnPenalty[rank];
        }

        // Passed pawn
        if ((oppositeSidePawns & passedPawnsMask) == default)
        {
            // Passed pawn without opponent pieces ahead (in its passed pawn mask)
            if ((passedPawnsMask & _occupancyBitBoards[oppositeSide]) == 0)
            {
                packedBonus += PassedPawnBonusNoEnemiesAheadBonus[bucket][rank];
                packedBonus += PassedPawnBonusNoEnemiesAheadEnemyBonus[oppositeSideBucket][rank];
            }

            // King distance to passed pawn
            var friendlyKingDistance = Constants.ChebyshevDistance[squareIndex][sameSideKingSquare];

            // Enemy king distance to passed pawn
            var enemyKingDistance = Constants.ChebyshevDistance[squareIndex][oppositeSideKingSquare];

            packedBonus += PassedPawnBonus[bucket][rank]
                + PassedPawnEnemyBonus[oppositeSideBucket][rank]
                + FriendlyKingDistanceToPassedPawnBonus[friendlyKingDistance]
                + EnemyKingDistanceToPassedPawnPenalty[enemyKingDistance];
        }

        // Pawn phalanx
        if (Constants.File[squareIndex] != 7 && _pieceBitBoards[pieceIndex].GetBit(squareIndex + 1))
        {
            packedBonus += PawnPhalanxBonus[rank];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int KnightAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToKnightOffset = (int)Piece.N - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToKnightOffset];

        var attacks = Attacks.KnightAttacks[squareIndex];
        evaluationContext.Attacks[(int)Piece.N + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = KnightMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += KnightKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToBishopOffset = (int)Piece.B - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToBishopOffset];

        var offset = Utils.PieceOffset(pieceSide);

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.BishopAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.B + offset] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = BishopMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += BishopKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        // Bad bishop
        var sameColorPawns = sameSidePawns &
            (Constants.DarkSquares[squareIndex] == 1
                ? Constants.DarkSquaresBitBoard
                : Constants.LightSquaresBitBoard);

        // Allowing playing positions with > 8 pawns
        var sameColorPawnsCount = sameColorPawns.CountBits() % 9;

        packedBonus += BadBishop_SameColorPawnsPenalty[sameColorPawnsCount];

        // Blocked central pawns
        var sameSideCentralPawns = sameSidePawns & Constants.CentralFiles;

        var pawnBlockerSquares = pieceSide == (int)Side.White
            ? sameSideCentralPawns.ShiftUp()
            : sameSideCentralPawns.ShiftDown();

        var pawnBlockers = pawnBlockerSquares & _occupancyBitBoards[Utils.OppositeSide(pieceSide)];

        packedBonus += BadBishop_BlockedCentralPawnsPenalty[pawnBlockers.CountBits()];

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
            if (squareIndex == (int)BoardSquare.a1 && _board[(int)BoardSquare.b2] == (int)Piece.P)
            {
                if (_board[(int)BoardSquare.b3] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }

            }
            else if (squareIndex == (int)BoardSquare.h1 && _board[(int)BoardSquare.g2] == (int)Piece.P)
            {
                if (_board[(int)BoardSquare.g3] == (int)Piece.None)
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

            if (squareIndex == (int)BoardSquare.a8 && _board[(int)BoardSquare.b7] == (int)Piece.p)
            {
                if (_board[(int)BoardSquare.b6] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
            else if (squareIndex == (int)BoardSquare.h8 && _board[(int)BoardSquare.g7] == (int)Piece.p)
            {
                if (_board[(int)BoardSquare.g6] == (int)Piece.None)
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
    private int RookAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToRookOffset];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.RookAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.R + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = RookMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += RookKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        var fileMask = Masks.FileMask(squareIndex);

        // Rook on open file
        if (((_pieceBitBoards[(int)Piece.P] | _pieceBitBoards[(int)Piece.p]) & fileMask) == default)
        {
            var file = Constants.File[squareIndex];
            packedBonus += OpenFileRookBonus[bucket][file];
            packedBonus += OpenFileRookEnemyBonus[oppositeSideBucket][file];
        }
        // Rook on semi-open file
        else if ((sameSidePawns & fileMask) == default)
        {
            var file = Constants.File[squareIndex];
            packedBonus += SemiOpenFileRookBonus[bucket][file];
            packedBonus += SemiOpenFileRookEnemyBonus[oppositeSideBucket][file];
        }

        // Connected rooks
        if ((attacks & _pieceBitBoards[pieceIndex]).CountBits() >= 1)
        {
            var rank = Constants.Rank[squareIndex];

            if (pieceIndex == (int)Piece.r)
            {
                rank = 7 - rank;
            }

            packedBonus += ConnectedRooksBonus[rank];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int QueenAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToQueenOffset = (int)Piece.Q - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToQueenOffset];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.QueenAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.Q + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = QueenMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += QueenKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int KingAdditionalEvaluation(int squareIndex, int bucket, int pieceSide, BitBoard enemyPawnAttacks)
    {
        // Virtual mobility (as if Queen)
        var attacksCount =
            (Attacks.QueenAttacks(squareIndex, _occupancyBitBoards[(int)Side.Both])
            & ~(_occupancyBitBoards[pieceSide] | enemyPawnAttacks)).CountBits();
        int packedBonus = VirtualKingMobilityBonus[attacksCount];

        var kingSideOffset = Utils.PieceOffset(pieceSide);

        // Opposite side rooks or queens on the board
        if (_pieceBitBoards[(int)Piece.r - kingSideOffset] + _pieceBitBoards[(int)Piece.q - kingSideOffset] != 0)
        {
            var file = Masks.FileMask(squareIndex);

            // King on open file
            if (((_pieceBitBoards[(int)Piece.P] | _pieceBitBoards[(int)Piece.p]) & file) == 0)
            {
                packedBonus += OpenFileKingPenalty[bucket][Constants.File[squareIndex]];
            }
            // King on semi-open file
            else if ((_pieceBitBoards[(int)Piece.P + kingSideOffset] & file) == 0)
            {
                packedBonus += SemiOpenFileKingPenalty[bucket][Constants.File[squareIndex]];
            }
        }

        // Pawn king shield included next to pawn additional eval

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int KingPawnShield(int squareIndex, BitBoard sameSidePawns, BitBoard oppositSidePawnAttacks)
    {
        var kingShield = Attacks.KingAttacks[squareIndex] & sameSidePawns;
        var kingShieldCount = kingShield.CountBits();

        var nonAttackedShieldCount = (kingShield & (~oppositSidePawnAttacks)).CountBits();

        return (KingShieldBonus * (kingShieldCount - nonAttackedShieldCount))
            + (KingShieldNonAttackedBonus * nonAttackedShieldCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PawnIslands(BitBoard whitePawns, BitBoard blackPawns)
    {
        var whiteIslandCount = CountPawnIslands(whitePawns);
        var blackIslandCount = CountPawnIslands(blackPawns);

        return PawnIslandsBonus[whiteIslandCount] - PawnIslandsBonus[blackIslandCount];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountPawnIslands(BitBoard pawns)
        {
            byte pawnFileBitBoard = 0;

            while (pawns != 0)
            {
                pawns = pawns.WithoutLS1B(out var squareIndex);

                // BitBoard.SetBit equivalent but for byte instead of ulong
                pawnFileBitBoard |= (byte)(1 << (squareIndex % 8));
            }

            int shifted = pawnFileBitBoard << 1;

            // Treat shifted’s MSB as 0 implicitly
            int starts = pawnFileBitBoard & (~shifted);

            return BitOperations.PopCount((uint)starts);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateThreats(ref EvaluationContext evaluationContext)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);
        ref var attacksBySideRef = ref MemoryMarshal.GetReference(evaluationContext.AttacksBySide);

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.White) |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.Black) |= existingAttacks;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Threats(EvaluationContext evaluationContext, int oppositeSide)
    {
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];
        int packedBonus = 0;

        var attacks = evaluationContext.Attacks;
        var board = _board;
        var defendedThreatsBonus = _defendedThreatsBonus;
        var undefendedThreatsBonus = _undefendedThreatsBonus;

        var defendedSquares = attacks[(int)Piece.P + oppositeSideOffset];

        for (int i = (int)Piece.N; i <= (int)Piece.K; ++i)
        {
            var threats = attacks[6 + i - oppositeSideOffset] & oppositeSidePieces;

            var defended = threats & defendedSquares;
            while (defended != 0)
            {
                defended = defended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += defendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }

            var undefended = threats & ~defendedSquares;
            while (undefended != 0)
            {
                undefended = undefended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += undefendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Checks(EvaluationContext evaluationContext, int side, int oppositeSide)
    {
        int packedBonus = 0;

        var offset = Utils.PieceOffset(side);
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        var oppositeSideKingSquare = _pieceBitBoards[(int)Piece.k - offset].GetLS1BIndex();
        var oppositeSideAttacks = evaluationContext.AttacksBySide[oppositeSide];

        Span<BitBoard> checkThreats = stackalloc BitBoard[5];

        var bishopAttacks = Attacks.BishopAttacks(oppositeSideKingSquare, occupancy);
        var rookAttacks = Attacks.RookAttacks(oppositeSideKingSquare, occupancy);

        checkThreats[(int)Piece.N] = Attacks.KnightAttacks[oppositeSideKingSquare];
        checkThreats[(int)Piece.B] = bishopAttacks;
        checkThreats[(int)Piece.R] = rookAttacks;
        checkThreats[(int)Piece.Q] = Attacks.QueenAttacks(rookAttacks, bishopAttacks);

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);

        for (int piece = (int)Piece.N; piece < (int)Piece.K; ++piece)
        {
            var checks = Unsafe.Add(ref attacksRef, piece + offset) & checkThreats[piece];
            var checksCount = checks.CountBits();

            var unsafeChecksCount = (checks & oppositeSideAttacks).CountBits();
            var safeChecksCount = checksCount - unsafeChecksCount;

            packedBonus += SafeCheckBonus[piece] * safeChecksCount;
            packedBonus += UnsafeCheckBonus[piece] * unsafeChecksCount;
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
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsBishopPawnDraw(BitBoard winningSidePawns, int winningSideOffset)
    {
        bool hasAFilePawn = (winningSidePawns & Constants.AFile) != 0;
        bool hasHFilePawn = (winningSidePawns & Constants.HFile) != 0;

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

        var defendingKingSquare = _pieceBitBoards[(int)Piece.k - winningSideOffset].GetLS1BIndex();

        // Not in the corner or adjacent squares
        if (Constants.ChebyshevDistance[defendingKingSquare][promotionCornerSquare] > 1)
        {
            return false;
        }

        var bishopSquare = _pieceBitBoards[(int)Piece.B + winningSideOffset].GetLS1BIndex();

        return BoardSquareExtensions.DifferentColor(bishopSquare, promotionCornerSquare);
    }

    /// <summary>
    /// If the pawn is in A or H files and the defending king reaches the corner/queening square, it's a draw.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsRookPawnDraw(BitBoard winningSidePawns, int winningSideOffset)
    {
        bool onlyAFilePawn = (winningSidePawns & Constants.AFile) == winningSidePawns;
        bool onlyHFilePawn = (winningSidePawns & Constants.HFile) == winningSidePawns;

        // Pawns not only in A or H files
        if (!(onlyAFilePawn || onlyHFilePawn))
        {
            return false;
        }

        // 1 if black is winning
        var inverseWinningSide = winningSideOffset >> 2;

        const int whiteBlackDiff = (int)BoardSquare.a1 - (int)BoardSquare.a8;

        var promotionCornerSquare =
            (onlyAFilePawn
                ? (int)BoardSquare.a8
                : (int)BoardSquare.h8)
            + (inverseWinningSide * whiteBlackDiff);

        var defendingKingSquare = _pieceBitBoards[(int)Piece.k - winningSideOffset].GetLS1BIndex();

        return Constants.ChebyshevDistance[defendingKingSquare][promotionCornerSquare] <= 1;
    }
}
