using Lynx.Model;
using NUnit.Framework;

using static Lynx.Test.MoveGeneration.MovePickerTest;

namespace Lynx.Test.MoveGeneration;

[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class MovePickerFRCTest : BaseTest
{
    public MovePickerFRCTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    /// <summary>
    /// Positions where castling is legal, in FRC/DFRC (castling is encoded as the KxR)
    /// </summary>
    private static readonly string[] _frcCastlingFens =
    [
        "r3k2r/8/8/8/8/8/8/R3K2R w AHah - 0 1",
        "r3k2r/8/8/8/8/8/8/R3K2R b AHah - 0 1",
        "5k2/8/8/8/8/8/8/1R1K3R w HB - 0 1",
        "1r1k3r/8/8/8/8/8/8/5K2 b hb - 0 1",
        "5k2/8/8/8/8/8/8/6KR w H - 0 1",                        // King doesn't move when castling short
        "5k2/8/8/8/8/8/8/RK6 w A - 0 1",                        // King moves 1 square, rook 2 when castling long
        "4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b EAhe - 0 1",
    ];

    private static readonly string[] _frcFens =
    [
        .. _frcCastlingFens,
        "bqnb1rkr/pp3ppp/3ppn2/2p5/5P2/P2P4/NPP1P1PP/BQ1BNRKR w HFhf - 2 9",
        "2nnrbkr/p1qppppp/8/1ppb4/6PP/3PP3/PPP2P2/BQNNRBKR w HEhe - 1 9",
        "2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1",
        "3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1",
        "r3k2r/8/8/8/8/8/4q3/R3K2R w AHah - 0 1",                                       // In check, castling not allowed
    ];

    [TestCaseSource(nameof(_frcFens))]
    public void MovePicker_FRC_ReturnsEveryLegalMoveOnceInScoreOrder(string fen)
    {
        AssertEveryLegalMoveIsReturnedOnceInScoreOrder(fen, _frcFens);
    }

    /// <summary>
    /// Castling in FRC/DFRC (KxR) as TT move, killer or counter move: returned exactly once,
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
