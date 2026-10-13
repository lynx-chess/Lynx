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

    private static void AssertEveryLegalMoveIsReturnedOnceInScoreOrder(string fen, string[] otherFens)
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

    private static Bitboard GetOppositeSideAttacks(Position position)
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);

        return evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];
    }

    private static Move[] GenerateAllMoves(Position position, Bitboard oppositeSideAttacks = 0)
    {
        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        return MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks).ToArray();
    }

    private static bool IsLegal(Position position, Move move)
    {
        var gameState = position.MakeMove(move);
        var isLegal = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return isLegal;
    }

    private static void SetKillersAndCounterMove(Engine engine, int ply, Move firstKiller, Move secondKiller, Move counterMove, Move previousMove)
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

    private static List<(Move Move, int Score)> GetAllMovesFromMovePicker(Engine engine, Position position, Bitboard oppositeSideAttacks, ShortMove ttMove, int ply, bool isQuiescence)
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

    /// <summary>
    /// FRC/DFRC: castling moves go to the rook square (king to rook square encoding).
    /// Needs <see cref="Configuration.EngineSettings.IsChess960"/>, which is global, so these run in isolation, see <see cref="Categories.Configuration"/>
    /// </summary>
    [Explicit]
    [Category(Categories.Configuration)]
    [NonParallelizable]
    public class FRC : BaseTest
    {
        private bool _previousIsChess960;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _previousIsChess960 = Configuration.EngineSettings.IsChess960;
            Configuration.EngineSettings.IsChess960 = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Configuration.EngineSettings.IsChess960 = _previousIsChess960;
        }

        /// <summary>
        /// Positions where castling is legal, in FRC/DFRC (castling is encoded as the king moving to its own rook square)
        /// </summary>
        private static readonly string[] _frcCastlingFens =
        [
            "r3k2r/8/8/8/8/8/8/R3K2R w AHah - 0 1",
            "r3k2r/8/8/8/8/8/8/R3K2R b AHah - 0 1",
            "5k2/8/8/8/8/8/8/1R1K3R w HB - 0 1",
            "1r1k3r/8/8/8/8/8/8/5K2 b hb - 0 1",
            "5k2/8/8/8/8/8/8/6KR w H - 0 1",                                                    // King doesn't move when castling short
            "5k2/8/8/8/8/8/8/RK6 w A - 0 1",                                                    // King moves 1 square, rook 2 when castling long
            "4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b EAhe - 0 1",
        ];

        private static readonly string[] _frcFens =
        [
            .. _frcCastlingFens,
            "bqnb1rkr/pp3ppp/3ppn2/2p5/5P2/P2P4/NPP1P1PP/BQ1BNRKR w HFhf - 2 9",
            "2nnrbkr/p1qppppp/8/1ppb4/6PP/3PP3/PPP2P2/BQNNRBKR w HEhe - 1 9",
            "2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1",
            "3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1",
        ];

        /// <summary>
        /// Same as <see cref="MovePicker_ReturnsEveryLegalMoveOnceInScoreOrder"/> for FRC/DFRC positions,
        /// where castling moves go to the rook square (and may even be the same as a king move in another position)
        /// </summary>
        [TestCaseSource(nameof(_frcFens))]
        public void MovePicker_FRC_ReturnsEveryLegalMoveOnceInScoreOrder(string fen)
        {
            AssertEveryLegalMoveIsReturnedOnceInScoreOrder(fen, _frcFens);
        }

        /// <summary>
        /// Castling in FRC/DFRC (king to rook square) as TT move, killer or counter move: returned exactly once,
        /// by the stage it belongs to, and with the same score as when generating all moves at once
        /// </summary>
        [TestCaseSource(nameof(_frcCastlingFens))]
        public void MovePicker_FRC_CastlingMove_IsReturnedOnceByItsStage(string fen)
        {
            var engine = GetEngine(fen);
            var position = engine.Game.CurrentPosition;
            var oppositeSideAttacks = GetOppositeSideAttacks(position);

            var allMoves = GenerateAllMoves(position, oppositeSideAttacks);
            var legalMoves = allMoves.Where(move => IsLegal(position, move)).Order().ToArray();
            var castlingMoves = allMoves.Where(move => move.IsCastle()).ToArray();

            Assert.IsNotEmpty(castlingMoves, $"No castling moves in {fen}");

            // Quiescence doesn't include castling
            Assert.IsEmpty(
                GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove: (ShortMove)castlingMoves[0], ply: 0, isQuiescence: true).Where(r => r.Move.IsCastle()),
                $"Castling in quiescence in {fen}");

            foreach (var castle in castlingMoves)
            {
                Assert.AreEqual((int)Piece.None, castle.CapturedPiece(), "Castling moves aren't captures, even if the king goes to the rook square");

                var scenarios = new (string Name, int Ply, ShortMove TTMove, Move FirstKiller, Move SecondKiller, Move CounterMove, int ExpectedScore)[]
                {
                    ("TT move", 0, (ShortMove)castle, 0, 0, 0, EvaluationConstants.TTMoveScoreValue),
                    ("first killer", 0, 0, castle, 0, 0, EvaluationConstants.FirstKillerMoveValue),
                    ("second killer", 0, 0, 0, castle, 0, EvaluationConstants.SecondKillerMoveValue),
                    ("counter move", 1, 0, 0, 0, castle, EvaluationConstants.CounterMoveValue),
                    ("not special", 1, 0, 0, 0, 0, engine.ScoreMove(position, castle, 1, oppositeSideAttacks, 0)),
                };

                foreach (var (name, ply, ttMove, firstKiller, secondKiller, counterMove, expectedScore) in scenarios)
                {
                    SetKillersAndCounterMove(engine, ply, firstKiller, secondKiller, counterMove, previousMove: allMoves[0]);

                    var returned = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove, ply, isQuiescence: false);
                    var description = $"{name}: {castle.UCIString()} in {fen}";

                    Assert.AreEqual(1, returned.Count(r => r.Move == castle), $"Castling not returned exactly once. {description}");
                    Assert.AreEqual(expectedScore, returned.Single(r => r.Move == castle).Score, $"Different castling score. {description}");
                    Assert.AreEqual(returned.Count, returned.Select(r => r.Move).Distinct().Count(), $"Repeated moves. {description}");
                    Assert.AreEqual(
                        legalMoves,
                        returned.Select(r => r.Move).Where(move => IsLegal(position, move)).Order().ToArray(),
                        $"Different legal moves. {description}");

                    if (name == "TT move")
                    {
                        Assert.AreEqual(castle, returned[0].Move, $"TT castling move not returned first. {description}");
                    }
                }
            }
        }
    }
}
