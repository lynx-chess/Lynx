using Lynx.Model;
using Xunit;

namespace Lynx.Test
{
    public class PositionTest
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN)]
        [InlineData(Constants.InitialPositionFEN)]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1")]
        [InlineData("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1")]
        [InlineData("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1")]
        public void FEN(string fen)
        {
            var position = new Position(fen);
            Assert.Equal(fen, position.FEN());

            var newPosition = new Position(position);
            Assert.Equal(fen, newPosition.FEN());
        }

        [Theory]
        [InlineData(Constants.EmptyBoardFEN)]
        [InlineData(Constants.InitialPositionFEN)]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1")]
        [InlineData("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1")]
        [InlineData("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 ")]
        public void CloneConstructor(string fen)
        {
            // Arrange
            var position = new Position(fen);

            // Act
            var clonedPosition = new Position(position);

            // Assert
            Assert.Equal(position.Side, clonedPosition.Side);
            Assert.Equal(position.Castle, clonedPosition.Castle);
            Assert.Equal(position.EnPassant, clonedPosition.EnPassant);

            for (int piece = 0; piece < position.PieceBitBoards.Length; ++piece)
            {
                Assert.Equal(position.PieceBitBoards[piece].Board, clonedPosition.PieceBitBoards[piece].Board);
            }

            for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
            {
                Assert.Equal(position.OccupancyBitBoards[occupancy].Board, clonedPosition.OccupancyBitBoards[occupancy].Board);
            }

            // Act: modify original, to ensure they're not sharing references to the same memory object
            for (int piece = 0; piece < position.PieceBitBoards.Length; ++piece)
            {
                position.PieceBitBoards[piece].ResetLS1B();
                position.PieceBitBoards[piece].SetBit((int)BoardSquare.e5 + piece);
            }

            for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
            {
                position.OccupancyBitBoards[occupancy].ResetLS1B();
                position.OccupancyBitBoards[occupancy].SetBit((int)BoardSquare.g7 + occupancy);
            }

            // Assert
            for (int piece = 0; piece < position.PieceBitBoards.Length; ++piece)
            {
                Assert.NotEqual(position.PieceBitBoards[piece].Board, clonedPosition.PieceBitBoards[piece].Board);
            }

            for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
            {
                Assert.NotEqual(position.OccupancyBitBoards[occupancy].Board, clonedPosition.OccupancyBitBoards[occupancy].Board);
            }
        }

        [Theory]
        [InlineData(Constants.InitialPositionFEN, true)]
        [InlineData(Constants.EmptyBoardFEN, false)]
        [InlineData("K/8/8/8/8/8/8/8 w - - 0 1", false)]
        [InlineData("K/8/8/8/8/8/8/8 b - - 0 1", false)]
        [InlineData("k/8/8/8/8/8/8/8 w - - 0 1", false)]
        [InlineData("k/8/8/8/8/8/8/8 b - - 0 1", false)]
        [InlineData("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 0 1", false)]
        [InlineData("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1", true)]
        [InlineData("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR b KQkq - 0 1", false)]
        [InlineData("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1", true)]
        public void IsValid(string fen, bool shouldBeValid)
        {
            Assert.Equal(shouldBeValid, new Position(fen).IsValid());
        }
    }
}
