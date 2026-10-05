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

    [TestCaseSource(nameof(_fens))]
    public void MovePicker_ReturnsSameMovesAsGeneratingAllMovesAtOnce(string fen)
    {
        AssertMovePickerMatchesGeneratingAllMovesAtOnce(fen, isQuiescence: false);
    }

    /// <summary>
    /// Only captures, promotions and castles, and quiet TT moves are ignored
    /// </summary>
    [TestCaseSource(nameof(_fens))]
    public void MovePicker_Quiescence_ReturnsSameMovesAsGeneratingAllCapturesAtOnce(string fen)
    {
        AssertMovePickerMatchesGeneratingAllMovesAtOnce(fen, isQuiescence: true);
    }

    private static void AssertMovePickerMatchesGeneratingAllMovesAtOnce(string fen, bool isQuiescence)
    {
        var engine = GetEngine(fen);
        var position = engine.Game.CurrentPosition;

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);
        var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var allMoves = (isQuiescence
            ? MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks)
            : MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks)).ToArray();

        // No TT move, and every pseudo-legal move as TT move (including quiets when testing quiescence)
        Span<Move> allMovesPool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var ttMoves = new List<ShortMove> { 0 };
        ttMoves.AddRange(MoveGenerator.GenerateAllMoves(position, allMovesPool, oppositeSideAttacks).ToArray().Select(move => (ShortMove)move).Distinct());

        foreach (var ttMove in ttMoves)
        {
            var expected = SortAlreadyGeneratedMoves(engine, position, allMoves, oppositeSideAttacks, ttMove, isQuiescence);
            var actual = GenerateMovesAndScoresUsingMovePicker(engine, position, oppositeSideAttacks, ttMove, isQuiescence);

            Assert.AreEqual(expected, actual, $"Different moves/order/scores for TT move {ttMove} in {fen}");
        }

        // Corrupted TT moves (promotion bits set for non-promotions) are ignored, instead of being returned as a different move
        var withoutTTMove = SortAlreadyGeneratedMoves(engine, position, allMoves, oppositeSideAttacks, 0, isQuiescence);

        foreach (var move in allMoves.Where(move => !move.IsPromotion() && move.Piece() != (int)Piece.P && move.Piece() != (int)Piece.p))
        {
            var corruptedTTMove = (ShortMove)(move | (int)Piece.Q);

            Assert.AreEqual(
                withoutTTMove,
                GenerateMovesAndScoresUsingMovePicker(engine, position, oppositeSideAttacks, corruptedTTMove, isQuiescence),
                $"Corrupted TT move {corruptedTTMove} in {fen}");
        }
    }

    private static List<(Move Move, int Score)> SortAlreadyGeneratedMoves(Engine engine, Position position, Move[] allMoves, Bitboard oppositeSideAttacks, ShortMove ttMove, bool isQuiescence)
    {
        var moves = allMoves.ToArray();
        var scores = moves.Select(move => isQuiescence
            ? engine.ScoreMoveQSearch(position, move, ttMove)
            : engine.ScoreMove(position, move, 0, oppositeSideAttacks, ttMove)).ToArray();

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

    private static List<(Move Move, int Score)> GenerateMovesAndScoresUsingMovePicker(Engine engine, Position position, Bitboard oppositeSideAttacks, ShortMove ttMove, bool isQuiescence)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var movePicker = new MovePicker(engine, position, ttMove, oppositeSideAttacks, 0, moves, moveScores, isQuiescence);

        var result = new List<(Move, int)>();

        while (movePicker.TryGetNext(out var move, out var score))
        {
            result.Add((move, score));
        }

        return result;
    }
}
