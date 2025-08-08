using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateCastlingMovesTest
{
#pragma warning disable RCS1098, S4144 // Methods should not have identical implementations

    [TestCase("1k6/8/8/8/8/8/RRRRRRRR/RN2K2R w KQ - 0 1")]
    [TestCase("7r/1k6/8/8/8/8/8/RN2K2R w KQ - 0 1", Description = "Attacking the rook")]
    [TestCase("rn2k2r/rrrrrrrr/8/8/8/8/8/1K6 b kq - 0 1")]
    [TestCase("rbnqk2r/8/8/8/8/8/1K6/7R b kq - 0 1", Description = "Attacking the rook")]
    public void ShortCastle(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.True(1 == moves.Count(m => m.IsCastle()));
        Assert.True(1 == moves.Count(m => m.IsShortCastle()));
        Assert.True(1 == moves.Count(m => m.IsCastle() && m.IsShortCastle()));
    }

    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/R3KBNR w KQ - 0 1")]
    [TestCase("r7/1k6/8/8/8/8/8/R3KBNR w KQ - 0 1", Description = "Attacking the rook")]
    [TestCase("1r6/1k6/8/8/8/8/8/R3KBNR w KQ - 0 1", Description = "Attacking square next to the rook")]
    [TestCase("r3k1nr/8/8/8/8/8/8/1K6 b kq - 0 1")]
    [TestCase("r3k1nr/8/8/8/8/8/1K6/R7 b kq - 0 1", Description = "Attacking the rook")]
    [TestCase("r3k1nr/8/8/8/8/8/1K6/1R6 b kq - 0 1", Description = "Attacking square next to the rook")]
    public void LongCastle(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.True(1 == moves.Count(m => m.IsCastle()));
        Assert.True(1 == moves.Count(m => m.IsLongCastle()));
        Assert.True(1 == moves.Count(m => m.IsCastle() && m.IsLongCastle()));
    }

    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w - - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b - - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qkq - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qk - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qq - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Q - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w q - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w k - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b KQq - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b KQ - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b Kq - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b K - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b Q - 0 1")]
    [TestCase("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b q - 0 1")]
    public void ShouldNotShortCastleWhenNoCastlingFlag(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsShortCastle()));
    }

    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w - - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b - - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kkq - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kk - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kq - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w K - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w k - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w q - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b KQk - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b KQ - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b Kk - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b K - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b Q - 0 1")]
    [TestCase("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b k - 0 1")]
    public void ShouldNotLongCastleWhenNoCastlingFlag(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsLongCastle()));
    }

    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQK1NR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQK1nR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQKB1R w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQKb1R w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQKbnR w KQkq - 0 1")]
    [TestCase("rnbqk1nr/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqk1Nr/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqkb1r/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqkB1r/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqkBNr/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    public void ShouldNotShortCastleWhenOccupiedSquares(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsShortCastle()));
    }

    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RN11K1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/Rn11K1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/R1B1K1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/R1b1K1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/R2QK1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/R2qK1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RNBQK1BR w KQkq - 0 1")]
    [TestCase("1k6/8/8/8/8/8/PPPPPPPP/RnbqK1BR w KQkq - 0 1")]
    [TestCase("rn2k1br/pppppppp/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rN2k1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("r1b1k1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("r1B1k1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("r2qk1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("r2Qk1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rnbqk1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    [TestCase("rNBQk1br/pppppppp/8/8/8/8/8/1K6 b KQkq - 0 1")]
    public void ShouldNotLongCastleWhenOccupiedSquares(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsLongCastle()));
    }

    [TestCase("4r3/1k6/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
    [TestCase("5r2/1k6/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
    [TestCase("6r1/1k6/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
    [TestCase("rnbqk2r/8/8/8/8/8/1K6/4R3 b KQkq - 0 1")]
    [TestCase("rnbqk2r/8/8/8/8/8/1K6/5R2 b KQkq - 0 1")]
    [TestCase("rnbqk2r/8/8/8/8/8/1K6/6R1 b KQkq - 0 1")]
    public void ShouldNotShortCastleWhenAttackedSquares(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsShortCastle()));
    }

    [TestCase("2r5/1k6/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
    [TestCase("3r4/1k6/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
    [TestCase("4r3/1k6/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
    [TestCase("r3qk1nr/8/8/8/8/8/1K6/2R5 b KQkq - 0 1")]
    [TestCase("r3qk1nr/8/8/8/8/8/1K6/3R4 b KQkq - 0 1")]
    [TestCase("r3qk1nr/8/8/8/8/8/1K6/4R3 b KQkq - 0 1")]
    public void ShouldNotLongCastleWhenAttackedSquares(string fen)
    {
        var position = new Position(fen);

        int index = 0;
        var moves = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        MoveGenerator.GenerateCastlingMoves(ref index, moves, position);

        Assert.IsEmpty(moves.Where(m => m.IsLongCastle()));
    }

#pragma warning restore RCS1098, S4144 // Methods should not have identical implementations
}
