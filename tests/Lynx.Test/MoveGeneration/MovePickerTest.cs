using Lynx.Model;
using NUnit.Framework;
using System.Runtime.Intrinsics;

namespace Lynx.Test.MoveGeneration;

public unsafe class MovePickerTest : BaseTest
{
    private static readonly string[] _inCheckFens =
    [
        "4k3/8/8/8/8/8/4q3/R3K2R w KQ - 0 1",                                           // Queen next to the king, castling not allowed
        "rnbqk1nr/pppp1ppp/8/4p3/1b1PP3/8/PPP2PPP/RNBQKBNR w KQkq - 1 3",               // Bishop check that can be blocked or captured
        "rnbqkbnr/pppppppp/8/8/8/5n2/PPPPPPPP/RNBQKB1R w KQkq - 0 1",                   // Knight check, can't be blocked
        "8/8/8/2k5/3Pp3/8/8/4K3 b - d3 0 1",                                            // Pawn check that can be captured en passant
        "4k3/8/8/8/8/3n4/8/r3K1R1 w - - 0 1",                                           // Double check: only king moves
        "7r/6P1/8/8/8/8/8/k6K w - - 0 1",                                               // Promotion capturing the checker
        "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",         // Not in check, used as reference below
    ];

    private static readonly string[] _fens =
    [
        .. _inCheckFens,
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.KillerTestPositionFEN,                                    // Promotions and en passant
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.CaptureTrainPositionFEN,
        "rnbqkbnr/ppp1p1pp/8/3pPp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3",    // En passant
        "4k3/8/8/8/8/8/4q3/R3K2R w KQ - 0 1",                               // In check, castling not allowed
        "r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1",
        "8/P1P5/8/8/8/8/p1p5/k1K5 w - - 0 1",                               // Promotions
    ];

    /// <summary>
    /// In check, only king moves, captures of the checker and blocks are returned.
    /// No pins are taken into account, and king moves could still be illegal.
    /// </summary>
    [TestCaseSource(nameof(_inCheckFens))]
    public void MovePicker_InCheck_ReturnsFewerMovesThanGeneratedAndAllTheLegalOnes(string fen)
    {
        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;
        var oppositeSideAttacks = GetOppositeSideAttacks(position);

        var allMoves = GenerateAllMoves(position, oppositeSideAttacks);
        var returned = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove: 0, ply: 0, isInCheck: position.IsInCheck(), capturesOnly: false);

        if (position.IsInCheck())
        {
            Assert.Less(returned.Count, allMoves.Length, $"Moves that don't get the king out of check weren't removed in {fen}");
        }
        else
        {
            Assert.AreEqual(allMoves.Length, returned.Count, $"Moves removed when not in check in {fen}");
        }

        Assert.AreEqual(
            allMoves.Where(move => IsLegal(position, move)).Order().ToArray(),
            returned.Select(r => r.Move).Where(move => IsLegal(position, move)).Order().ToArray(),
            $"Different legal moves in {fen}");
    }

    /// <summary>
    /// Quiescence, built the way <see cref="Engine.QuiescenceSearch"/> does it:
    /// when in check all the evasions are returned (captures and quiet moves) and nothing else, otherwise only captures and promotions
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void MovePicker_Quiescence_ReturnsEvasionsWhenInCheckAndCapturesOtherwise(string fen)
    {
        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;
        var isInCheck = position.IsInCheck();
        var oppositeSideAttacks = GetOppositeSideAttacks(position);

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var captures = MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks).ToArray();
        var allMoves = GenerateAllMoves(position, oppositeSideAttacks);

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var movePicker = new MovePicker(engine, position, ttMove: 0, oppositeSideAttacks, ply: 0, moves, moveScores, isInCheck: isInCheck, capturesOnly: !isInCheck);

        var returned = new List<Move>();
        while (movePicker.TryGetNext(out var move, out _))
        {
            returned.Add(move);
        }

        var expected = isInCheck ? allMoves : captures;

        Assert.That(returned, Is.SubsetOf(expected), $"Returned something unexpected in {fen}");
        Assert.AreEqual(returned.Count, returned.Distinct().Count(), $"Repeated moves in {fen}");
        Assert.AreEqual(
            expected.Where(move => IsLegal(position, move)).Order().ToArray(),
            returned.Where(move => IsLegal(position, move)).Order().ToArray(),
            $"Different legal moves in {fen}");

        if (isInCheck)
        {
            Assert.LessOrEqual(returned.Count, allMoves.Length, $"More moves than generated in {fen}");
        }
        else
        {
            Assert.AreEqual(captures.Length, returned.Count, $"Captures removed when not in check in {fen}");
        }
    }

    /// <summary>
    /// Quiescence: only captures and promotions, in the same order and with the same scores as sorting them all at once.
    /// Quiet TT moves are ignored, and so are 'corrupted' ones (aka the result of hashing collisions).
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
            var actual = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove, ply: 0, isInCheck: false, capturesOnly: true);

            Assert.AreEqual(expected, actual, $"Different moves/order/scores for TT move {ttMove} in {fen}");
        }

        var withoutTTMove = SortAlreadyGeneratedMoves(engine, position, captures, 0);

        foreach (var move in captures.Where(move => !move.IsPromotion() && move.Piece() != (int)Piece.P && move.Piece() != (int)Piece.p))
        {
            var corruptedTTMove = (ShortMove)(move | (int)Piece.Q);

            Assert.AreEqual(
                withoutTTMove,
                GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, corruptedTTMove, ply: 0, isInCheck: false, capturesOnly: true),
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

                var returned = GetAllMovesFromMovePicker(engine, position, oppositeSideAttacks, ttMove, ply, isInCheck: position.IsInCheck(), capturesOnly: false);

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

    public enum IndexOfFirstMaxImplementation
    {
        Scalar,
        Vector128,
        Vector256,
        Vector512,

        /// <summary>
        /// <see cref="MovePicker.IndexOfFirstMax"/>, which picks one of the others depending on the hardware
        /// </summary>
        HardwareDependent
    }

    private static readonly (int Min, int Max)[] _indexOfFirstMaxScoreRanges =
    [
        (0, 0),
        (0, 3),
        (-1_000, 1_000),
        (int.MinValue, int.MaxValue),
    ];

    /// <summary>
    /// Every <see cref="MovePicker.IndexOfFirstMax"/> implementation returns the first index of the highest score, like a scalar search with strict '>',
    /// for every count and start, with many ties and with extreme values.
    /// All of them run on any hardware (vectors are only slower when they aren't accelerated), so they're all tested in every environment.
    /// Each implementation only gets the padding (<see cref="int.MinValue"/>) it needs after count, and the scores after it are <see cref="int.MaxValue"/>,
    /// so reading past the padding would return a wrong index
    /// </summary>
    [Test]
    public void IndexOfFirstMax_ReturnsTheSameIndexAsAScalarSearch(
        [Values] IndexOfFirstMaxImplementation implementation,
        [ValueSource(nameof(_indexOfFirstMaxScoreRanges))] (int Min, int Max) scoreRange)
    {
        var (minScore, maxScore) = scoreRange;

        var paddingLength = IndexOfFirstMaxPaddingLength(implementation);

        var random = new Random(42);

        for (int count = 1; count <= 64; ++count)
        {
            for (int iteration = 0; iteration < 10; ++iteration)
            {
                var scores = new int[count + paddingLength + 16];
                Array.Fill(scores, int.MaxValue);
                Array.Fill(scores, int.MinValue, count, paddingLength);

                for (int i = 0; i < count; ++i)
                {
                    scores[i] = random.Next(8) == 0
                        ? (random.Next(2) == 0 ? minScore : maxScore)
                        : (int)random.NextInt64(minScore, (long)maxScore + 1);
                }

                var description = $"count {count}, scores [{string.Join(", ", scores[..count])}]";

                for (int start = 0; start < count; ++start)
                {
                    var expected = start;

                    for (int i = start + 1; i < count; ++i)
                    {
                        if (scores[i] > scores[expected])
                        {
                            expected = i;
                        }
                    }

                    var actual = IndexOfFirstMax(implementation, scores, start, count);

                    Assert.AreEqual(expected, actual, $"start {start}, {description}");
                }
            }
        }
    }

    /// <summary>
    /// The vectorized <see cref="MovePicker.IndexOfFirstMax"/> implementations look for the highest score 64 scores at a time,
    /// so this checks lists of up to 4 of those chunks, with the first highest score in every position (and a tie after it), from every start up to it
    /// </summary>
    [Test]
    public void IndexOfFirstMax_FindsTheHighestScoreAfterTheFirst64Scores([Values] IndexOfFirstMaxImplementation implementation)
    {
        const int count = 218;      // Most legal moves in a chess position
        const int highestScore = 1_000;

        var paddingLength = IndexOfFirstMaxPaddingLength(implementation);

        var random = new Random(42);

        for (int highestScoreIndex = 0; highestScoreIndex < count; ++highestScoreIndex)
        {
            var scores = new int[count + paddingLength + 16];
            Array.Fill(scores, int.MaxValue);
            Array.Fill(scores, int.MinValue, count, paddingLength);

            for (int i = 0; i < count; ++i)
            {
                scores[i] = random.Next(-highestScore, highestScore);
            }

            scores[highestScoreIndex] = highestScore;

            if (highestScoreIndex + 1 < count)
            {
                scores[random.Next(highestScoreIndex + 1, count)] = highestScore;
            }

            for (int start = 0; start <= highestScoreIndex; ++start)
            {
                Assert.AreEqual(highestScoreIndex, IndexOfFirstMax(implementation, scores, start, count), $"start {start}");
            }
        }
    }

    /// <summary>
    /// <see cref="int.MinValue"/> scores that <paramref name="implementation"/> needs after count
    /// </summary>
    private static int IndexOfFirstMaxPaddingLength(IndexOfFirstMaxImplementation implementation) => implementation switch
    {
        IndexOfFirstMaxImplementation.Scalar => 0,
        IndexOfFirstMaxImplementation.Vector128 => Vector128<int>.Count - 1,
        IndexOfFirstMaxImplementation.Vector256 => Vector256<int>.Count - 1,
        IndexOfFirstMaxImplementation.Vector512 => Vector512<int>.Count - 1,
        IndexOfFirstMaxImplementation.HardwareDependent => Vector512<int>.Count - 1,   // The most any implementation needs
        _ => throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null)
    };

    private static int IndexOfFirstMax(IndexOfFirstMaxImplementation implementation, ReadOnlySpan<int> scores, int start, int count) => implementation switch
    {
        IndexOfFirstMaxImplementation.Scalar => MovePicker.IndexOfFirstMaxScalar(scores, start, count),
        IndexOfFirstMaxImplementation.Vector128 => MovePicker.IndexOfFirstMaxVector128(scores, start, count),
        IndexOfFirstMaxImplementation.Vector256 => MovePicker.IndexOfFirstMaxVector256(scores, start, count),
        IndexOfFirstMaxImplementation.Vector512 => MovePicker.IndexOfFirstMaxVector512(scores, start, count),
        IndexOfFirstMaxImplementation.HardwareDependent => MovePicker.IndexOfFirstMax(scores, start, count),
        _ => throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null)
    };

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
        engine._killerMoves.Clear();
        engine._counterMoves.Clear();

        engine._killerMoves.Pointer[ply * 2] = firstKiller;
        engine._killerMoves.Pointer[(ply * 2) + 1] = secondKiller;

        if (ply >= 1)
        {
            engine.Game.Stack(ply - 1).Move = previousMove;
            engine._counterMoves.Pointer[(previousMove.Piece() * 64) + previousMove.TargetSquare()] = counterMove;
        }
    }

    private static List<(Move Move, int Score)> SortAlreadyGeneratedMoves(Engine engine, Position position, Move[] generatedMoves, ShortMove ttMove)
    {
        var moves = generatedMoves.ToArray();
        var scores = moves.Select(move => engine.ScoreMoveQSearch(position, move, ttMove)).ToArray();

        var result = new List<(Move, int)>(moves.Length);

        for (int moveIndex = 0; moveIndex < moves.Length; ++moveIndex)
        {
            var bestIndex = moveIndex;

            for (int j = moveIndex + 1; j < moves.Length; j++)
            {
                if (scores[j] > scores[bestIndex])
                {
                    bestIndex = j;
                }
            }

            (scores[moveIndex], scores[bestIndex], moves[moveIndex], moves[bestIndex]) = (scores[bestIndex], scores[moveIndex], moves[bestIndex], moves[moveIndex]);

            result.Add((moves[moveIndex], scores[moveIndex]));
        }

        return result;
    }

    internal static List<(Move Move, int Score)> GetAllMovesFromMovePicker(Engine engine, Position position, Bitboard oppositeSideAttacks, ShortMove ttMove, int ply, bool isInCheck, bool capturesOnly)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var movePicker = new MovePicker(engine, position, ttMove, oppositeSideAttacks, ply, moves, moveScores, isInCheck: isInCheck, capturesOnly: capturesOnly);

        var result = new List<(Move, int)>();

        while (movePicker.TryGetNext(out var move, out var score))
        {
            result.Add((move, score));
        }

        return result;
    }
}
