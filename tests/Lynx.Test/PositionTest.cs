using Lynx.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lynx.Test
{
    public class PositionTest
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN)]
        [InlineData(Constants.InitialPositionFEN)]
        [InlineData(Constants.TrickyTestPositionFEN)]
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
        [InlineData(Constants.TrickyTestPositionFEN)]
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

        [Theory]
        [InlineData(Constants.InitialPositionFEN, true)]
        [InlineData(Constants.EmptyBoardFEN, false)]
        [InlineData("K/8/8/8/8/8/8/8 w - - 0 1", false)]
        [InlineData("K/8/8/8/8/8/8/8 b - - 0 1", true)]
        [InlineData("k/8/8/8/8/8/8/8 w - - 0 1", true)]
        [InlineData("k/8/8/8/8/8/8/8 b - - 0 1", false)]
        [InlineData("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 0 1", false)]
        [InlineData("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1", true)]
        [InlineData("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR b KQkq - 0 1", false)]
        [InlineData("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1", true)]
        public void WasProduceByAValidMove(string fen, bool shouldBeValid)
        {
            Assert.Equal(shouldBeValid, new Position(fen).WasProduceByAValidMove());
        }

        [Theory]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", +100)]
        [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", +100)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNBQKBNR w KQkq - 0 1", -100)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNBQKBNR b KQkq - 0 1", -100)]
        [InlineData("rnbqkb1r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", +300)]
        [InlineData("rnbqkb1r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", +300)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKB1R w KQkq - 0 1", -300)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKB1R b KQkq - 0 1", -300)]
        [InlineData("rnbqk1nr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", +350)]
        [InlineData("rnbqk1nr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", +350)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK1NR w KQkq - 0 1", -350)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK1NR b KQkq - 0 1", -350)]
        [InlineData("rnbqkbn1/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", +500)]
        [InlineData("rnbqkbn1/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", +500)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBN1 w KQkq - 0 1", -500)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBN1 b KQkq - 0 1", -500)]
        [InlineData("rnb1kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", +900)]
        [InlineData("rnb1kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", +900)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNB1KBNR w KQkq - 0 1", -900)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNB1KBNR b KQkq - 0 1", -900)]
        public void EvaluateMaterial(string fen, int expectedEvaluationValue)
        {
            Assert.Equal(expectedEvaluationValue, new Position(fen).EvaluateMaterial());
        }

        [Theory]
        [InlineData("7k/8/8/8/8/3B4/1K6/6Q1 b - - 0 1", 0)]
        [InlineData("7K/8/8/8/8/3b4/1k6/6q1 w - - 0 1", 0)]
        [InlineData("8/5K2/7p/6pk/6p1/6P1/7P/8 b - - 0 1", 0)]
        [InlineData("8/7p/6p1/6P1/6PK/5k1P/8/8 w - - 0 1", 0)]
        [InlineData("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", +Position.CheckMateEvaluation)]
        [InlineData("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -Position.CheckMateEvaluation)]
        public void EvaluateFinalPosition_AlphaBeta(string fen, int expectedEvaluationValue)
        {
            // Arrange
            var position = new Position(fen);
            Assert.Empty(position.AllPossibleMoves().Where(move => new Position(position, move).IsValid()));

            // Act
            var noDepthResult = position.EvaluateFinalPosition_AlphaBeta(default);
            var depthOneResult = position.EvaluateFinalPosition_AlphaBeta(1);
            var depthTwoResult = position.EvaluateFinalPosition_AlphaBeta(2);

            Assert.Equal(expectedEvaluationValue, noDepthResult);

            if (expectedEvaluationValue > 0)
            {
                Assert.Equal(Side.Black, position.Side);

                Assert.True(noDepthResult > depthOneResult);
                Assert.True(depthOneResult > depthTwoResult);
            }
            else if (expectedEvaluationValue < 0)
            {
                Assert.Equal(Side.White, position.Side);

                Assert.True(noDepthResult < depthOneResult);
                Assert.True(depthOneResult < depthTwoResult);
            }
        }

        [Theory]
        [InlineData("7k/8/8/8/8/3B4/1K6/6Q1 b - - 0 1", 0)]
        [InlineData("7K/8/8/8/8/3b4/1k6/6q1 w - - 0 1", 0)]
        [InlineData("8/5K2/7p/6pk/6p1/6P1/7P/8 b - - 0 1", 0)]
        [InlineData("8/7p/6p1/6P1/6PK/5k1P/8/8 w - - 0 1", 0)]
        [InlineData("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", -Position.CheckMateEvaluation)]
        [InlineData("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -Position.CheckMateEvaluation)]
        public void EvaluateFinalPosition_NegaMax(string fen, int expectedEvaluationValue)
        {
            // Arrange
            var position = new Position(fen);
            Assert.Empty(position.AllPossibleMoves().Where(move => new Position(position, move).IsValid()));

            // Act
            var noDepthResult = position.EvaluateFinalPosition_NegaMax(default, new());
            var depthOneResult = position.EvaluateFinalPosition_NegaMax(1, new());
            var depthTwoResult = position.EvaluateFinalPosition_NegaMax(2, new());

            if (expectedEvaluationValue < 0)
            {
                Assert.Equal(expectedEvaluationValue, noDepthResult);

                Assert.True(noDepthResult < depthOneResult);
                Assert.True(depthOneResult < depthTwoResult);
            }
        }

        [Fact]
        public void EvaluateFinalPosition_NegaMax_Threefold()
        {
            var winningPosition = new Position("7k/8/5KR1/8/8/8/5R2/K7 w - - 0 1");

            var game = new Game(winningPosition);
            var repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                new ((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                new ((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),   // Triple position repetition
                new ((int)BoardSquare.f2, (int)BoardSquare.a2, (int)Piece.R),   // Other random move
                new ((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.f2, (int)BoardSquare.h2, (int)Piece.R)    // Mate
            };

            repeatedMoves.ForEach(move => Assert.True(game.MakeMove(move)));

            Assert.Equal(repeatedMoves.Count, game.MoveHistory.Count);

            var eval = winningPosition.EvaluateMaterialAndPosition_NegaMax(game.PositionFENHistory);
            Assert.Equal(0, eval);
        }

        [Fact]
        public void EvaluateMaterialAndPosition_NegaMax_Threefold()
        {
            // https://lichess.org/MgWVifcK
            var winningPosition = new Position("6k1/6b1/1p6/2p5/P7/1K4R1/8/r7 b - - 7 52");

            var game = new Game(winningPosition);
            var repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                new ((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                new ((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                new ((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),
                new ((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                new ((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                new ((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                new ((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),
            };

            repeatedMoves.ForEach(move => Assert.True(game.MakeMove(move)));
            Assert.Equal(repeatedMoves.Count, game.MoveHistory.Count);

            var eval = winningPosition.EvaluateMaterialAndPosition_NegaMax(game.PositionFENHistory);
            Assert.Equal(0, eval);
        }
    }
}
