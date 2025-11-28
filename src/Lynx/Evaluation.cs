using System.Runtime.CompilerServices;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public partial class Position
{
    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, int Phase) StaticEvaluation()
    {
        int packedScore = 0;
        int gamePhase = 0;

        if (IsIncrementalEval)
        {
            packedScore = IncrementalEvalAccumulator;
            gamePhase = IncrementalPhaseAccumulator;
        }
        else
        {
            IncrementalEvalAccumulator = 0;
            IncrementalPhaseAccumulator = 0;

            for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    IncrementalEvalAccumulator += PSQT(pieceIndex, pieceSquareIndex);

                    IncrementalPhaseAccumulator += TunableEvalParameters.GamePhaseByPiece[pieceIndex];
                }
            }

            packedScore += IncrementalEvalAccumulator;
            gamePhase += IncrementalPhaseAccumulator;
            IsIncrementalEval = true;
        }

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        int endGamePhase = MaxPhase - gamePhase;

        var middleGameScore = Utils.UnpackMG(packedScore);
        var endGameScore = Utils.UnpackEG(packedScore);
        var eval = ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / MaxPhase;

        eval = Math.Clamp(eval, MinStaticEval, MaxStaticEval);

        var sideEval = _side == Side.White
            ? eval
            : -eval;

        return (sideEval, gamePhase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Phase()
    {
        int gamePhase =
             ((_pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.N])
            + ((_pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.B])
            + ((_pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.R])
            + ((_pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.Q]);

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        return gamePhase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PhaseFromScratch()
    {
        return (Knights.CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.N])
            + (Bishops.CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.B])
            + (Rooks.CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.R])
            + (Queens.CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.Q]);
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

    public void CalculateThreats(ref EvaluationContext evaluationContext)
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
    }
}
