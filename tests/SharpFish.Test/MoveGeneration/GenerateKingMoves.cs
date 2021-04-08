using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateKingMoves
    {
        [Theory]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/8/2PPP3/2PKP3/2P1P3/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/3PP3/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/2PP4/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/3P4/8/8 w - - 0 1", 2)]
        [InlineData("8/8/8/2PPP3/2PKP3/8/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2PPP3/2PK4/8/8/8 w - - 0 1", 4)]
        [InlineData("8/8/8/2PPP3/3K4/8/8/8 w - - 0 1", 5)]
        [InlineData("8/8/8/2P1P3/3K4/8/8/8 w - - 0 1", 6)]
        [InlineData("8/8/8/4P3/3K4/8/8/8 w - - 0 1", 7)]
        [InlineData("8/8/8/8/3K4/8/8/8 w - - 0 1", 8)]
        public void NonCapturingMoves(string fen, int expectedMoves)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var moves = MovesGenerator.GenerateKingMoves(game, offset);

            Assert.Equal(expectedMoves, moves.Count(m =>
                m.Piece == (int)Piece.K + offset
                && m.MoveType == MoveType.Quiet));
        }

        [Theory]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/8/2ppp3/2pKp3/2p1p3/8/8 w - - 0 1", 5)]
        [InlineData("8/8/8/2ppp3/2pKp3/3pp3/8/8 w - - 0 1", 4)]
        [InlineData("8/8/8/2ppp3/2pKp3/2pp4/8/8 w - - 0 1", 4)]
        [InlineData("8/8/8/2ppp3/2pKp3/3p4/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2ppp3/2pKp3/8/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2ppp3/2pK4/8/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2ppp3/3K4/8/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2p1p3/3K4/8/8/8 w - - 0 1", 2)]
        [InlineData("8/8/8/4p3/3K4/8/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/8/3K4/8/8/8 w - - 0 1", 0)]
        [InlineData("8/8/8/2nnn3/2nKn3/2nnn3/8/8 w - - 0 1", 0)]
        [InlineData("8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", 0)]
        public void CapturingMoves(string fen, int expectedMoves)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var moves = MovesGenerator.GenerateKingMoves(game, offset);

            Assert.Equal(expectedMoves, moves.Count(m =>
                m.Piece == (int)Piece.K + offset
                && m.MoveType == MoveType.Capture));
        }
    }
}
