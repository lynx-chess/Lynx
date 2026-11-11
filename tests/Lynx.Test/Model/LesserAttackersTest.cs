using Lynx.Model;
using Lynx.Test.MoveGeneration;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class LesserAttackersTest : BaseTest
{
    private const int Minor = EvaluationConstants.LesserAttackerMinorScore;
    private const int Rook = EvaluationConstants.LesserAttackerRookScore;
    private const int Queen = EvaluationConstants.LesserAttackerQueenScore;

    /// <summary>
    /// Every one of them has at least one quiet move with a non-zero <see cref="LesserAttackers.Score(Move)"/>
    /// </summary>
    private static readonly string[] _fensWithLesserAttackers =
    [
        "4k3/3p4/8/4p3/3N4/8/8/4K3 w - - 0 1",                                          // Knight attacked by a pawn, two of its moves go to squares attacked by another pawn
        "7k/8/8/2n5/4R3/8/8/4K3 w - - 0 1",                                             // Rook attacked by a knight
        "3r4/7k/8/8/3Q4/8/8/4K3 w - - 0 1",                                             // Queen attacked by a rook
        Constants.TrickyTestPositionFEN,                                                // Knight attacked by a pawn
        "rnbqkb1r/ppp1pppp/5n2/3pP3/8/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 3",              // Black knight attacked by a pawn
        "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 5 4",          // Quiet moves to squares attacked by pawns, minors
    ];

    private static readonly string[] _fens =
    [
        .. _fensWithLesserAttackers,
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.KillerTestPositionFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.CaptureTrainPositionFEN,
        "8/P1P5/8/8/8/8/p1p5/k1K5 w - - 0 1",
        "4k3/8/8/8/8/8/4q3/R3K2R w KQ - 0 1",                                           // In check, castling not allowed
        "rnbqk1nr/pppp1ppp/8/4p3/1b1PP3/8/PPP2PPP/RNBQKBNR w KQkq - 1 3",               // In check, can be blocked
    ];

    [Test]
    public void Score_Knight()
    {
        var scores = ScoreQuietMoves("4k3/3p4/8/4p3/3N4/8/8/4K3 w - - 0 1");

        // d4 is attacked by the e5 pawn
        Assert.AreEqual(8, scores.Count(kvp => kvp.Key.StartsWith("d4")));

        foreach (var escape in new[] { "d4b3", "d4b5", "d4c2", "d4e2", "d4f3", "d4f5" })
        {
            Assert.AreEqual(Minor, scores[escape], escape);
        }

        // c6 and e6 are attacked by the d7 pawn
        foreach (var attacked in new[] { "d4c6", "d4e6" })
        {
            Assert.AreEqual(-Minor, scores[attacked], attacked);
        }

        AssertAllZero(scores, "e1");
    }

    [Test]
    public void Score_Bishop()
    {
        // Bishop attacked by the e5 pawn
        var scores = ScoreQuietMoves("4k3/8/8/4p3/3B4/8/8/4K3 w - - 0 1");

        Assert.AreEqual(Minor, scores["d4c3"]);
        Assert.AreEqual(Minor, scores["d4e3"]);
        Assert.AreEqual(Minor, scores["d4c5"]);
        Assert.AreEqual(Minor, scores["d4g1"]);
        Assert.AreEqual(Minor, scores["d4a7"]);

        // Bishop not attacked, d4 and f4 are attacked by the e5 pawn
        scores = ScoreQuietMoves("4k3/8/8/4p3/8/4B3/8/4K3 w - - 0 1");

        Assert.AreEqual(-Minor, scores["e3d4"]);
        Assert.AreEqual(-Minor, scores["e3f4"]);
        Assert.AreEqual(0, scores["e3d2"]);
        Assert.AreEqual(0, scores["e3f2"]);
        Assert.AreEqual(0, scores["e3c5"]);
        Assert.AreEqual(0, scores["e3g5"]);
    }

    [Test]
    public void Score_Rook()
    {
        // The knight on c5 attacks the rook, a4 and e6
        var scores = ScoreQuietMoves("7k/8/8/2n5/4R3/8/8/4K3 w - - 0 1");

        foreach (var attacked in new[] { "e4a4", "e4e6" })
        {
            Assert.AreEqual(-Rook, scores[attacked], attacked);
        }

        foreach (var escape in new[] { "e4b4", "e4c4", "e4d4", "e4f4", "e4g4", "e4h4", "e4e2", "e4e3", "e4e5", "e4e7", "e4e8" })
        {
            Assert.AreEqual(Rook, scores[escape], escape);
        }

        AssertAllZero(scores, "e1");
    }

    [Test]
    public void Score_Queen()
    {
        // The rook on d8 attacks the queen, the d-file and the 8th rank
        var scores = ScoreQuietMoves("3r4/7k/8/8/3Q4/8/8/4K3 w - - 0 1");

        foreach (var attacked in new[] { "d4d5", "d4d6", "d4d7", "d4h8" })
        {
            Assert.AreEqual(-Queen, scores[attacked], attacked);
        }

        foreach (var escape in new[] { "d4a4", "d4c4", "d4g4", "d4h4", "d4e5", "d4f6", "d4g7", "d4c5", "d4b6", "d4a7", "d4e3", "d4f2", "d4g1", "d4c3", "d4b2", "d4a1" })
        {
            Assert.AreEqual(Queen, scores[escape], escape);
        }

        AssertAllZero(scores, "e1");
    }

    /// <summary>
    /// Known limitation, see <see cref="LesserAttackers"/>: the attacks are the ones of the current position, so they don't see through the moving piece.
    /// A queen that retreats along the line it's attacked from is scored as an escape, although the attacker will still hit it
    /// (a rook can't be attacked by a lesser slider along a line it can move through, so it isn't affected).
    /// This pins that behavior: if the x-ray is ever taken into account, the squares behind the queen should score -Queen and this test should be updated.
    /// Moving towards the attacker, on the other hand, is scored as moving to an attacked square.
    /// </summary>
    [TestCase("3r4/7k/8/8/3Q4/8/8/4K3 w - - 0 1", "d4d3 d4d2 d4d1", "d4d5 d4d6 d4d7", Description = "Queen attacked by a rook along the d-file")]
    [TestCase("8/7k/8/8/r2Q4/8/8/4K3 w - - 0 1", "d4e4 d4f4 d4g4 d4h4", "d4c4 d4b4", Description = "Queen attacked by a rook along the 4th rank")]
    [TestCase("k7/6b1/8/8/3Q4/8/8/4K3 w - - 0 1", "d4c3 d4b2 d4a1", "d4e5 d4f6", Description = "Queen attacked by a bishop along the long diagonal")]
    public void Score_RetreatingAlongTheAttackersRay_IsScoredAsEscape(string fen, string retreats, string approaches)
    {
        var scores = ScoreQuietMoves(fen);

        foreach (var retreat in retreats.Split(' '))
        {
            Assert.AreEqual(Queen, scores[retreat], $"{retreat}: the queen is still attacked after moving there");
        }

        foreach (var approach in approaches.Split(' '))
        {
            Assert.AreEqual(-Queen, scores[approach], approach);
        }
    }

    /// <summary>
    /// Pieces attacked by a piece of the same value or lower, or by the king, aren't considered to be in danger
    /// </summary>
    [TestCase("8/6bk/8/8/3N4/8/8/4K3 w - - 0 1", "d4", Description = "Knight attacked by a bishop")]
    [TestCase("7k/8/8/8/r3R3/8/8/4K3 w - - 0 1", "e4", Description = "Rook attacked by a rook")]
    [TestCase("8/3q3k/8/8/3Q4/8/8/4K3 w - - 0 1", "d4", Description = "Queen attacked by a queen")]
    [TestCase("1r5k/8/8/8/3N4/8/8/4K3 w - - 0 1", "d4", Description = "Knight moving to squares (b3, b5) attacked by a rook")]
    public void Score_AttackedByEqualOrGreaterPiece_Zero(string fen, string startSquare)
    {
        var scores = ScoreQuietMoves(fen);

        var pieceMoves = scores.Where(kvp => kvp.Key.StartsWith(startSquare)).ToList();
        Assert.IsNotEmpty(pieceMoves);

        AssertAllZero(scores, startSquare);
    }

    /// <summary>
    /// Pawns can't be attacked by a lesser piece, and kings can't be captured
    /// </summary>
    [Test]
    public void Score_PawnsAndKings_Zero()
    {
        // e4 is attacked by the d5 pawn, and the king moves around
        var scores = ScoreQuietMoves("7k/8/8/3p4/8/8/4P3/4K3 w - - 0 1");

        Assert.Contains("e2e4", scores.Keys);
        Assert.Contains("e1d1", scores.Keys);

        Assert.IsTrue(scores.Values.All(score => score == 0));
    }

    [TestCaseSource(nameof(_fens))]
    public void Score_Default_Zero(string fen)
    {
        var scores = ScoreQuietMoves(fen, useAttacks: false);

        Assert.IsTrue(scores.Values.All(score => score == 0));
    }

    /// <summary>
    /// The score of a move must not depend on the color of the side moving.
    /// Catches things like adding the piece number, which is offset by 6 for black.
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void Score_IsColorSymmetric(string fen)
    {
        var mirroredFen = MirrorFEN(fen);

        var scores = ScoreQuietMoves(fen);
        var mirroredScores = ScoreQuietMoves(mirroredFen);

        Assert.AreEqual(scores.Count, mirroredScores.Count, $"Different number of quiet moves in {fen} and {mirroredFen}");

        foreach (var (move, score) in scores)
        {
            Assert.AreEqual(score, mirroredScores[MirrorUCIMove(move)], $"{move} in {fen} vs {MirrorUCIMove(move)} in {mirroredFen}");
        }
    }

    [TestCaseSource(nameof(_fensWithLesserAttackers))]
    public void Score_FensWithLesserAttackers_AreNotVacuous(string fen)
    {
        Assert.IsTrue(ScoreQuietMoves(fen).Values.Any(score => score != 0), fen);
    }

    /// <summary>
    /// <see cref="MovePicker"/> only adds <see cref="LesserAttackers.Score(Move)"/> to quiet moves that aren't TT, killer nor counter moves.
    /// Noisy moves, their order and their scores, as well as the rest of the quiet moves, remain the same.
    /// Both pickers get the same attacks of the side, so the same moves are generated.
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void MovePicker_AddsLesserAttackersScoreToQuietMoves(string fen)
    {
        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;
        var oppositeSideAttacks = MovePickerTest.GetOppositeSideAttacks(position);
        var attacksByPiece = GetOppositeSideAttacksByPiece(position);
        var lesserAttackers = new LesserAttackers(attacksByPiece);

        var without = GetMovesFromMovePicker(engine, position, oppositeSideAttacks);
        var with = GetMovesFromMovePicker(engine, position, attacksByPiece);

        Assert.AreEqual(without.Count, with.Count, $"Different number of moves in {fen}");

        // Noisy moves
        Assert.AreEqual(
            without.Where(m => !IsQuiet(m.Move)).ToList(),
            with.Where(m => !IsQuiet(m.Move)).ToList(),
            $"Noisy moves changed in {fen}");

        var quietWithout = without.Where(m => IsQuiet(m.Move)).ToDictionary(m => m.Move, m => m.Score);
        var quietWith = with.Where(m => IsQuiet(m.Move)).ToList();

        Assert.AreEqual(quietWithout.Count, quietWith.Count, fen);

        var changed = 0;
        foreach (var (move, score) in quietWith)
        {
            var expectedScore = quietWithout[move] + lesserAttackers.Score(move);
            Assert.AreEqual(expectedScore, score, $"{move.UCIString()} in {fen}");

            if (score != quietWithout[move])
            {
                ++changed;
            }
        }

        if (_fensWithLesserAttackers.Contains(fen))
        {
            Assert.Greater(changed, 0, $"No quiet move score changed in {fen}");
        }

        for (int i = 1; i < quietWith.Count; ++i)
        {
            Assert.GreaterOrEqual(quietWith[i - 1].Score, quietWith[i].Score, $"Quiet moves not sorted at {i} in {fen}");
        }
    }

    /// <summary>
    /// With no history, the queen escaping the rook goes before everything else, and moving it to a square attacked by the rook goes after
    /// </summary>
    [Test]
    public void MovePicker_QuietMovesOrder()
    {
        const string fen = "3r4/7k/8/8/3Q4/8/8/4K3 w - - 0 1";

        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;

        var moves = GetMovesFromMovePicker(engine, position, GetOppositeSideAttacksByPiece(position))
            .Where(m => IsQuiet(m.Move))
            .Select(m => m.Move.UCIString())
            .ToList();

        var queenEscape = moves.IndexOf("d4g4");
        var kingMove = moves.IndexOf("e1d1");
        var queenToAttackedSquare = moves.IndexOf("d4d5");

        Assert.GreaterOrEqual(queenEscape, 0);
        Assert.GreaterOrEqual(kingMove, 0);
        Assert.GreaterOrEqual(queenToAttackedSquare, 0);

        Assert.Less(queenEscape, kingMove);
        Assert.Less(kingMove, queenToAttackedSquare);

        // The only quiet moves to squares attacked by the rook are the last ones
        foreach (var attacked in new[] { "d4d5", "d4d6", "d4d7", "d4h8" })
        {
            Assert.GreaterOrEqual(moves.IndexOf(attacked), moves.Count - 4, attacked);
        }
    }

    /// <summary>
    /// <see cref="NegaMax"/> uses either <see cref="Position.StaticEvaluation(int, PawnTableElement[], ref EvaluationContext)"/> or
    /// <see cref="Position.CalculateThreats(ref EvaluationContext)"/> to get the attacks depending on the node, and <see cref="MovePicker"/> relies on both of them
    /// producing the same attacks by piece, and those adding up to the attacks by side.
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void AttacksByPiece_AreTheSameInBothPathsAndAddUpToAttacksBySide(string fen)
    {
        var position = GetEngine(fen).Game.CurrentPosition;

        var (staticEvaluationAttacks, staticEvaluationAttacksBySide) = GetAttacks(position, useStaticEvaluation: true);
        var (threatsAttacks, threatsAttacksBySide) = GetAttacks(position, useStaticEvaluation: false);

        Assert.AreEqual(threatsAttacks, staticEvaluationAttacks, $"Different attacks by piece in {fen}");
        Assert.AreEqual(threatsAttacksBySide, staticEvaluationAttacksBySide, $"Different attacks by side in {fen}");

        foreach (var side in new[] { (int)Side.Black, (int)Side.White })
        {
            Bitboard union = 0;
            foreach (var attacks in threatsAttacks.AsSpan(Utils.PieceOffset(side), (int)Piece.K + 1))
            {
                union |= attacks;
            }

            Assert.AreEqual(threatsAttacksBySide[side], union, $"Attacks by piece don't add up to the attacks of side {side} in {fen}");
        }
    }

    private static (Bitboard[] Attacks, Bitboard[] AttacksBySide) GetAttacks(Position position, bool useStaticEvaluation)
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        if (useStaticEvaluation)
        {
            position.StaticEvaluation(0, new PawnTableElement[Constants.KingPawnHashSize], ref evaluationContext);
        }
        else
        {
            position.CalculateThreats(ref evaluationContext);
        }

        return (evaluationContext.Attacks.ToArray(), evaluationContext.AttacksBySide.ToArray());
    }

    private static Bitboard[] GetOppositeSideAttacksByPiece(Position position)
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);

        return evaluationContext.Attacks
            .Slice(Utils.PieceOffset(Utils.OppositeSide((int)position.Side)), (int)Piece.K + 1)
            .ToArray();
    }

    /// <summary>
    /// UCI move -> <see cref="LesserAttackers.Score(Move)"/> for all the quiet moves of the position
    /// </summary>
    private static Dictionary<string, int> ScoreQuietMoves(string fen, bool useAttacks = true)
    {
        var position = GetEngine(fen).Game.CurrentPosition;
        var oppositeSideAttacks = MovePickerTest.GetOppositeSideAttacks(position);

        var lesserAttackers = useAttacks
            ? new LesserAttackers(GetOppositeSideAttacksByPiece(position))
            : default;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var result = new Dictionary<string, int>();
        foreach (var move in MoveGenerator.GenerateAllQuiets(position, movePool, oppositeSideAttacks))
        {
            result.Add(move.UCIString(), lesserAttackers.Score(move));
        }

        Assert.IsNotEmpty(result, $"No quiet moves in {fen}");

        return result;
    }

    /// <summary>
    /// Picker built from the attacks of the side, without <see cref="LesserAttackers"/>
    /// </summary>
    private static List<(Move Move, int Score)> GetMovesFromMovePicker(Engine engine, Position position, Bitboard oppositeSideAttacks)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var movePicker = new MovePicker(engine, position, ttMove: 0, oppositeSideAttacks, ply: 0, moves, moveScores, isInCheck: position.IsInCheck(), capturesOnly: false);

        return GetAllMoves(ref movePicker);
    }

    /// <summary>
    /// Picker built from the attacks of the side and the ones of each of its pieces, with <see cref="LesserAttackers"/>
    /// </summary>
    private static List<(Move Move, int Score)> GetMovesFromMovePicker(Engine engine, Position position, ReadOnlySpan<Bitboard> oppositeSideAttacksByPiece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var oppositeSideAttacks = MovePickerTest.GetOppositeSideAttacks(position);

        var movePicker = new MovePicker(engine, position, ttMove: 0, oppositeSideAttacks, oppositeSideAttacksByPiece, ply: 0, moves, moveScores, isInCheck: position.IsInCheck());

        return GetAllMoves(ref movePicker);
    }

    private static List<(Move Move, int Score)> GetAllMoves(ref MovePicker movePicker)
    {
        var result = new List<(Move, int)>();

        while (movePicker.TryGetNext(out var move, out var score))
        {
            result.Add((move, score));
        }

        return result;
    }

    private static bool IsQuiet(Move move) => move.CapturedPiece() == (int)Piece.None && move.PromotedPiece() == default;

    private static void AssertAllZero(Dictionary<string, int> scores, string startSquare)
    {
        var moves = scores.Where(kvp => kvp.Key.StartsWith(startSquare)).ToList();

        Assert.IsNotEmpty(moves, $"No moves from {startSquare}");

        foreach (var (move, score) in moves)
        {
            Assert.AreEqual(0, score, move);
        }
    }

    /// <summary>
    /// Flips the board vertically, swaps the colors and the side to move
    /// </summary>
    private static string MirrorFEN(string fen)
    {
        var parts = fen.Split(' ');

        var board = string.Join('/', parts[0].Split('/').Reverse().Select(SwapCase));
        var side = parts[1] == "w" ? "b" : "w";
        var castling = parts[2] == "-"
            ? "-"
            : new string([.. SwapCase(parts[2]).OrderBy(c => "KQkq".IndexOf(c))]);
        var enPassant = parts[3] == "-"
            ? "-"
            : $"{parts[3][0]}{9 - (parts[3][1] - '0')}";

        // Clocks (if any) stay the same
        return string.Join(' ', new List<string> { board, side, castling, enPassant }.Concat(parts.Skip(4)));

        static string SwapCase(string value) =>
            new([.. value.Select(c => char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c))]);
    }

    private static string MirrorUCIMove(string uciMove) =>
        $"{uciMove[0]}{9 - (uciMove[1] - '0')}{uciMove[2]}{9 - (uciMove[3] - '0')}{uciMove[4..]}";
}
