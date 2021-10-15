﻿using Lynx.Model;
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
        [InlineData(Constants.TrickyPositionReversedFEN)]
        [InlineData(Constants.KillerPositionFEN)]
        [InlineData("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1")]
        public void FEN(string fen)
        {
            var position = new Position(fen);
            Assert.Equal(fen, position.FEN);

            var newPosition = new Position(position);
            Assert.Equal(fen, newPosition.FEN);
        }

        [Theory]
        [InlineData(Constants.EmptyBoardFEN)]
        [InlineData(Constants.InitialPositionFEN)]
        [InlineData(Constants.TrickyTestPositionFEN)]
        [InlineData(Constants.TrickyPositionReversedFEN)]
        [InlineData(Constants.KillerPositionFEN)]
        [InlineData(Constants.CmkPositionFEN)]
        public void CloneConstructor(string fen)
        {
            // Arrange
            var position = new Position(fen);

            // Act
            var clonedPosition = new Position(position);

            // Assert
            Assert.Equal(position.FEN, clonedPosition.FEN);
            Assert.Equal(position.UniqueIdentifier, clonedPosition.UniqueIdentifier);
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
        [InlineData("r1k5/1K6/8/8/8/8/8/8 w - - 0 1", false)]
        public void IsValid(string fen, bool shouldBeValid)
        {
            Assert.Equal(shouldBeValid, new Position(fen).IsValid());
        }

        [Fact]
        public void CustomIsValid()
        {
            var origin = new Position("r2k4/1K6/8/8/8/8/8/8 b - - 0 1");
            var move = new Move((int)BoardSquare.b7, (int)BoardSquare.a8, (int)Piece.K, isCapture: 1);

            var newPosition = new Position(origin, move);
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
            var noDepthResult = position.EvaluateFinalPosition(default, new(), default);
            var depthOneResult = position.EvaluateFinalPosition(1, new(), default);
            var depthTwoResult = position.EvaluateFinalPosition(2, new(), default);

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

            var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
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

            var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
            Assert.Equal(0, eval);
        }

        [Fact]
        public void EvaluateMaterialAndPosition_NegaMax_Threefold_CastleRightsRemoval()
        {
            // Arrange

            // Position without castling rights
            var winningPosition = new Position("1n2k2r/8/8/8/8/8/4PPPP/1N2K2R w - - 0 1");

            var game = new Game(winningPosition);
            var repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                new ((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                new ((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                new ((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),
                new ((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                new ((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                new ((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                new ((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)
            };

            repeatedMoves.ForEach(move => Assert.True(game.MakeMove(move)));
            Assert.Equal(repeatedMoves.Count, game.MoveHistory.Count);

            var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
            Assert.Equal(0, eval);

            // Position with castling rights, lost in move Ke1d1
            winningPosition = new Position("1n2k2r/8/8/8/8/8/4PPPP/1N2K2R w Kk - 0 1");

            game = new Game(winningPosition);
            repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                new ((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                new ((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                new ((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),
                new ((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                new ((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                new ((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                new ((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)
            };

            // Act
            repeatedMoves.ForEach(move => Assert.True(game.MakeMove(move)));
            Assert.Equal(repeatedMoves.Count, game.MoveHistory.Count);

            eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
            Assert.NotEqual(0, eval);

            repeatedMoves.TakeLast(4).ToList().ForEach(move => Assert.True(game.MakeMove(move)));
            Assert.Equal(repeatedMoves.Count + 4, game.MoveHistory.Count);
            eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
            Assert.NotEqual(0, eval);

            repeatedMoves.TakeLast(4).ToList().ForEach(move => Assert.True(game.MakeMove(move)));
            Assert.Equal(repeatedMoves.Count + 8, game.MoveHistory.Count);
            eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
            Assert.Equal(0, eval);
        }

        [Fact]
        public void EvaluateFinalPosition_NegaMax_50MovesRule()
        {
            var winningPosition = new Position("7k/8/5KR1/8/8/8/5R2/K7 w - - 0 1");

            var game = new Game(winningPosition);
            var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                new ((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k)
            };

            for (int i = 0; i < 48; ++i)
            {
                Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
            }

            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[0]));
            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[1]));
            Assert.True(game.MakeMove(new Move((int)BoardSquare.e2, (int)BoardSquare.h2, (int)Piece.R)));   // Mate on move 51

            Assert.Equal(51, game.MoveHistory.Count);

            var eval = winningPosition.StaticEvaluation(new(), game.MovesWithoutCaptureOrPawnMove);
            Assert.Equal(0, eval);
        }

        [Fact]
        public void EvaluateMaterialAndPosition_NegaMax_50MovesRule()
        {
            // https://lichess.org/MgWVifcK
            var winningPosition = new Position("6k1/6b1/1p6/2p5/P7/1K4R1/8/r7 b - - 7 52");

            var game = new Game(winningPosition);
            var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                new ((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                new ((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                new ((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                new ((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K)
            };

            for (int i = 0; i < 50; ++i)
            {
                Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
            }

            Assert.Equal(50, game.MoveHistory.Count);

            var eval = winningPosition.StaticEvaluation(new(), game.MovesWithoutCaptureOrPawnMove);
            Assert.Equal(0, eval);
        }

        [Fact]
        public void EvaluateMaterialAndPosition_NegaMax_50MovesRule_Promotion()
        {
            // https://lichess.org/MgWVifcK
            var winningPosition = new Position("6k1/6b1/1p6/2p5/P7/1K4R1/7p/r7 b - - 7 52");

            var game = new Game(winningPosition);
            var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                new ((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                new ((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                new ((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                new ((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K)
            };

            for (int i = 0; i < 48; ++i)
            {
                Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
            }

            Assert.True(game.MakeMove(new((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)(Piece.q))));   // Promotion
            Assert.True(game.MakeMove(new((int)BoardSquare.b3, (int)BoardSquare.c4, (int)Piece.K)));
            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[2]));

            Assert.Equal(51, game.MoveHistory.Count);

            var eval = winningPosition.StaticEvaluation(new(), game.MovesWithoutCaptureOrPawnMove);
            Assert.NotEqual(0, eval);
        }
    }
}
