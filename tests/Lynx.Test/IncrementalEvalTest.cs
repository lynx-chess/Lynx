using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;
public class IncrementalEvalTest
{
    /// <summary>
    /// If castling moves are ever refactored, i.e. when adding FRC support, this should break.
    /// That'll mean that incremental eval condition in <see cref="Position.MakeMove(int)"/> needs to change
    /// </summary>
    [Test]
    public void CastlingMovesAreKingMoves()
    {
        Assert.AreEqual((int)Piece.K, IMoveGenerator.WhiteShortCastle.Piece());
        Assert.AreEqual((int)Piece.K, IMoveGenerator.WhiteLongCastle.Piece());
        Assert.AreEqual((int)Piece.k, IMoveGenerator.BlackShortCastle.Piece());
        Assert.AreEqual((int)Piece.k, IMoveGenerator.BlackLongCastle.Piece());
    }
}
