using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateCastlingMovesTest
    {
#pragma warning disable S4144 // Methods should not have identical implementations

        [Theory]
        [InlineData("8/8/8/8/8/8/RRRRRRRR/RN2K2R w KQkq - 0 1")]
        [InlineData("7r/8/8/8/8/8/8/RN2K2R w KQkq - 0 1")]          // Attacking the rook
        [InlineData("rn2k2r/rrrrrrrr/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rbnqk2r/8/8/8/8/8/8/7R b KQkq - 0 1")]         // Attacking the rook
        public void ShortCastle(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Single(moves.Where(m => m.IsCastle() && m.IsShortCastle()));
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1")]
        [InlineData("r7/8/8/8/8/8/8/R3KBNR w KQkq - 0 1")]      // Attacking the rook
        [InlineData("1r6/8/8/8/8/8/8/R3KBNR w KQkq - 0 1")]     // Attacking square next to the rook
        [InlineData("r3k1nr/8/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("r3k1nr/8/8/8/8/8/8/R7 b KQkq - 0 1")]      // Attacking the rook
        [InlineData("r3k1nr/8/8/8/8/8/8/1R6 b KQkq - 0 1")]     // Attacking square next to the rook
        public void LongCastle(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Single(moves.Where(m => m.IsCastle() && m.IsLongCastle()));
        }

        [Theory]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w - - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b - - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qkq - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qk - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Qq - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w Q - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w q - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w k - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b KQq - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b KQ - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b Kq - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b K - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b Q - 0 1")]
        [InlineData("rbnqk2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R b q - 0 1")]
        public void ShouldNotShortCastleWhenNoCastlingFlag(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsShortCastle()));
        }

        [Theory]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w - - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b - - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kkq - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kk - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w Kq - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w K - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w k - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w q - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b KQk - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b KQ - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b Kk - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b K - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b Q - 0 1")]
        [InlineData("r3k1nr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b k - 0 1")]
        public void ShouldNotLongCastleWhenNoCastlingFlag(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsLongCastle()));
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQK1NR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQK1nR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQKB1R w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQKb1R w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQKbnR w KQkq - 0 1")]
        [InlineData("rnbqk1nr/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqk1Nr/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqkb1r/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqkB1r/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqkBNr/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        public void ShouldNotShortCastleWhenOccupiedSquares(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsShortCastle()));
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RN11K1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/Rn11K1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/R1B1K1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/R1b1K1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/R2QK1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/R2qK1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RNBQK1BR w KQkq - 0 1")]
        [InlineData("8/8/8/8/8/8/PPPPPPPP/RnbqK1BR w KQkq - 0 1")]
        [InlineData("rn2k1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rN2k1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("r1b1k1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("r1B1k1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("r2qk1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("r2Qk1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rnbqk1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]
        [InlineData("rNBQk1br/pppppppp/8/8/8/8/8/8 b KQkq - 0 1")]

        public void ShouldNotLongCastleWhenOccupiedSquares(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsLongCastle()));
        }

        [Theory]
        [InlineData("4r3/8/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
        [InlineData("5r2/8/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
        [InlineData("6r1/8/8/8/8/8/8/RNBQK2R w KQkq - 0 1")]
        [InlineData("rnbqk2r/8/8/8/8/8/8/4R3 b KQkq - 0 1")]
        [InlineData("rnbqk2r/8/8/8/8/8/8/5R2 b KQkq - 0 1")]
        [InlineData("rnbqk2r/8/8/8/8/8/8/6R1 b KQkq - 0 1")]
        public void ShouldNotShortCastleWhenAttackedSquares(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsShortCastle()));
        }

        [Theory]
        [InlineData("2r5/8/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
        [InlineData("3r4/8/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
        [InlineData("4r3/8/8/8/8/8/8/R3K1NR w KQkq - 0 1")]
        [InlineData("r3qk1nr/8/8/8/8/8/8/2R5 b KQkq - 0 1")]
        [InlineData("r3qk1nr/8/8/8/8/8/8/3R4 b KQkq - 0 1")]
        [InlineData("r3qk1nr/8/8/8/8/8/8/4R3 b KQkq - 0 1")]
        public void ShouldNotLongCastleWhenAttackedSquares(string fen)
        {
            var position = new Position(fen);

            var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side));

            Assert.Empty(moves.Where(m => m.IsCastle() && m.IsLongCastle()));
        }

#pragma warning restore S4144 // Methods should not have identical implementations
    }
}
