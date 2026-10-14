using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class MovePickerTest : BaseTest
{
    private static readonly string[] _fens =
    [
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.KillerTestPositionFEN, // Promotions and en passant
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.CaptureTrainPositionFEN,
        "rnbqkbnr/ppp1p1pp/8/3pPp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3", // En passant
        "4k3/8/8/8/8/8/4q3/R3K2R w KQ - 0 1", // In check, castling not allowed
        "r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1",
        "8/P1P5/8/8/8/8/p1p5/k1K5 w - - 0 1", // Promotions
    ];

    /// <summary>
    /// Quiescence: only captures and promotions, in the same order and with the same scores as sorting them all at once.
    /// Quiet TT moves are ignored, and so are corrupted ones.
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void MovePicker_Quiescence_ReturnsSameMovesAsGeneratingAllCapturesAtOnce(string fen)
    {
        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;
        var oppositeSideAttacks = GetOppositeSideAttacks(position);

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var captures = MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks).ToArray();
        var allMoves = GenerateAllMoves(position, oppositeSideAttacks);

        // No TT move, and every pseudo-legal move as TT move (including quiets, which must be ignored)
        var ttMoves = new List<ShortMove> { 0 };
        ttMoves.AddRange(allMoves.Select(move => (ShortMove)move).Distinct());

        foreach (var ttMove in ttMoves)
        {
            var expected = SortAlreadyGeneratedMoves(engine, position, captures, ttMove);
            var actual = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove, ply: 0, isQuiescence: true);

            Assert.AreEqual(expected, actual, $"Different moves/order/scores for TT move {ttMove} in {fen}");
        }

        var withoutTTMove = SortAlreadyGeneratedMoves(engine, position, captures, 0);

        foreach (var move in captures.Where(move => !move.IsPromotion() && move.Piece() != (int)Piece.P && move.Piece() != (int)Piece.p))
        {
            var corruptedTTMove = (ShortMove)(move | (int)Piece.Q);

            Assert.AreEqual(
                withoutTTMove,
                GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, corruptedTTMove, ply: 0, isQuiescence: true),
                $"Corrupted TT move {corruptedTTMove} in {fen}");
        }
    }

    /// <summary>
    /// Staged generation: every legal move is returned exactly once, with the score <see cref="Engine.ScoreMove"/> would give it,
    /// and scores never increase, whatever the TT move, killers and counter move are (valid, stale, repeated, missing).
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void MovePicker_ReturnsEveryLegalMoveOnceInScoreOrder(string fen)
    {
        AssertEveryLegalMoveIsReturnedOnceInScoreOrder(fen, _fens);
    }

    internal static void AssertEveryLegalMoveIsReturnedOnceInScoreOrder(string fen, string[] otherFens)
    {
        var random = new Random(42);

        foreach (var ply in new[] { 0, 1 })
        {
            var engine = GetEngine(fen);
            var position = engine.Game.CurrentPosition;
            var oppositeSideAttacks = GetOppositeSideAttacks(position);

            var allMoves = GenerateAllMoves(position, oppositeSideAttacks);
            var legalMoves = allMoves.Where(move => IsLegal(position, move)).Order().ToArray();
            var quietMoves = allMoves.Where(move => move.CapturedPiece() == (int)Piece.None && !move.IsPromotion()).ToArray();

            // Quiet moves from other positions, as stale killers/counter moves would be
            var staleQuietMoves = otherFens
                .Where(otherFen => otherFen != fen)
                .SelectMany(otherFen => GenerateAllMoves(new Position(otherFen)))
                .Where(move => move.CapturedPiece() == (int)Piece.None && !move.IsPromotion())
                .ToArray();

            Move PickQuiet(params Move[] repeated)
            {
                return random.Next(5) switch
                {
                    0 => 0,
                    1 when staleQuietMoves.Length > 0 => staleQuietMoves[random.Next(staleQuietMoves.Length)],
                    2 when repeated.Length > 0 => repeated[random.Next(repeated.Length)],
                    _ => quietMoves.Length > 0 ? quietMoves[random.Next(quietMoves.Length)] : 0
                };
            }

            for (int i = 0; i < 100; ++i)
            {
                // TT move: none, any move (noisy or quiet), or from another position
                var ttMove = (ShortMove)(random.Next(4) switch
                {
                    0 => 0,
                    1 => staleQuietMoves[random.Next(staleQuietMoves.Length)],
                    _ => allMoves[random.Next(allMoves.Length)]
                });

                var firstKiller = PickQuiet();
                var secondKiller = PickQuiet(firstKiller, ttMove);
                var counterMove = PickQuiet(firstKiller, secondKiller, ttMove);

                SetKillersAndCounterMove(engine, ply, firstKiller, secondKiller, counterMove, previousMove: allMoves[0]);

                var description = $"TT: {ttMove}, killers: {firstKiller}, {secondKiller}, counter: {counterMove}, ply: {ply}, FEN: {fen}";

                var returned = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove, ply, isQuiescence: false);

                Assert.AreEqual(returned.Count, returned.Select(r => r.Move).Distinct().Count(), $"Repeated moves. {description}");

                // Illegal moves can be returned too (e.g. king moves to attacked squares coming from killers), search discards them
                Assert.AreEqual(
                    legalMoves,
                    returned.Select(r => r.Move).Where(move => IsLegal(position, move)).Order().ToArray(),
                    $"Different legal moves. {description}");

                foreach (var (move, score) in returned)
                {
                    Assert.AreEqual(engine.ScoreMove(position, move, ply, oppositeSideAttacks, ttMove), score, $"Different score for {move}. {description}");
                }

                for (int j = 1; j < returned.Count; ++j)
                {
                    Assert.GreaterOrEqual(returned[j - 1].Score, returned[j].Score, $"Scores not sorted at {j}. {description}");
                }
            }
        }
    }

    internal static Bitboard GetOppositeSideAttacks(Position position)
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);

        return evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];
    }

    internal static Move[] GenerateAllMoves(Position position, Bitboard oppositeSideAttacks = 0)
    {
        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        return MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks).ToArray();
    }

    internal static bool IsLegal(Position position, Move move)
    {
        var gameState = position.MakeMove(move);
        var isLegal = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return isLegal;
    }

    internal static void SetKillersAndCounterMove(Engine engine, int ply, Move firstKiller, Move secondKiller, Move counterMove, Move previousMove)
    {
        Array.Clear(engine._killerMoves);
        Array.Clear(engine._counterMoves);

        engine._killerMoves[ply * 2] = firstKiller;
        engine._killerMoves[(ply * 2) + 1] = secondKiller;

        if (ply >= 1)
        {
            engine.Game.Stack(ply - 1).Move = previousMove;
            engine._counterMoves[(previousMove.Piece() * 64) + previousMove.TargetSquare()] = counterMove;
        }
    }

    private static List<(Move Move, int Score)> SortAlreadyGeneratedMoves(Engine engine, Position position, Move[] generatedMoves, ShortMove ttMove)
    {
        var moves = generatedMoves.ToArray();
        var scores = moves.Select(move => engine.ScoreMoveQSearch(position, move, ttMove)).ToArray();

        var result = new List<(Move, int)>(moves.Length);

        for (int moveIndex = 0; moveIndex < moves.Length; ++moveIndex)
        {
            for (int j = moveIndex + 1; j < moves.Length; j++)
            {
                if (scores[j] > scores[moveIndex])
                {
                    (scores[moveIndex], scores[j], moves[moveIndex], moves[j]) = (scores[j], scores[moveIndex], moves[j], moves[moveIndex]);
                }
            }

            result.Add((moves[moveIndex], scores[moveIndex]));
        }

        return result;
    }

    internal static List<(Move Move, int Score)> GetAllMovesFromMovePicker(Engine engine, Position position, Bitboard oppositeSideAttacks, ShortMove ttMove, int ply, bool isQuiescence)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var movePicker = new MovePicker(engine, position, ttMove, oppositeSideAttacks, ply, moves, moveScores, isQuiescence);

        var result = new List<(Move, int)>();

        while (movePicker.TryGetNext(out var move, out var score))
        {
            result.Add((move, score));
        }

        return result;
    }
}
