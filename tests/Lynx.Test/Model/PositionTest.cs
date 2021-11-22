using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class PositionTest
{
    [TestCase(Constants.EmptyBoardFEN)]
    [TestCase(Constants.InitialPositionFEN)]
    [TestCase(Constants.TrickyTestPositionFEN)]
    [TestCase(Constants.TrickyTestPositionReversedFEN)]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1")]
    public void FEN(string fen)
    {
        var position = new Position(fen);
        Assert.AreEqual(fen, position.FEN);

        var newPosition = new Position(position);
        Assert.AreEqual(fen, newPosition.FEN);
    }

    [TestCase(Constants.EmptyBoardFEN)]
    [TestCase(Constants.InitialPositionFEN)]
    [TestCase(Constants.TrickyTestPositionFEN)]
    [TestCase(Constants.TrickyTestPositionReversedFEN)]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase(Constants.CmkTestPositionFEN)]
    public void CloneConstructor(string fen)
    {
        // Arrange
        var position = new Position(fen);

        // Act
        var clonedPosition = new Position(position);

        // Assert
        Assert.AreEqual(position.FEN, clonedPosition.FEN);
        Assert.AreEqual(position.UniqueIdentifier, clonedPosition.UniqueIdentifier);
        Assert.AreEqual(position.Side, clonedPosition.Side);
        Assert.AreEqual(position.Castle, clonedPosition.Castle);
        Assert.AreEqual(position.EnPassant, clonedPosition.EnPassant);

        for (int piece = 0; piece < position.PieceBitBoards.Length; ++piece)
        {
            Assert.AreEqual(position.PieceBitBoards[piece].Board, clonedPosition.PieceBitBoards[piece].Board);
        }

        for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
        {
            Assert.AreEqual(position.OccupancyBitBoards[occupancy].Board, clonedPosition.OccupancyBitBoards[occupancy].Board);
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
            Assert.AreNotEqual(position.PieceBitBoards[piece].Board, clonedPosition.PieceBitBoards[piece].Board);
        }

        for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
        {
            Assert.AreNotEqual(position.OccupancyBitBoards[occupancy].Board, clonedPosition.OccupancyBitBoards[occupancy].Board);
        }
    }

    [TestCase(Constants.InitialPositionFEN, true)]
    [TestCase(Constants.EmptyBoardFEN, false)]
    [TestCase("K/8/8/8/8/8/8/8 w - - 0 1", false)]
    [TestCase("K/8/8/8/8/8/8/8 b - - 0 1", false)]
    [TestCase("k/8/8/8/8/8/8/8 w - - 0 1", false)]
    [TestCase("k/8/8/8/8/8/8/8 b - - 0 1", false)]
    [TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 0 1", false)]
    [TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1", true)]
    [TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR b KQkq - 0 1", false)]
    [TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1", true)]
    [TestCase("r1k5/1K6/8/8/8/8/8/8 w - - 0 1", false)]
    public void IsValid(string fen, bool shouldBeValid)
    {
        Assert.AreEqual(shouldBeValid, new Position(fen).IsValid());
    }

    [Test]
    public void CustomIsValid()
    {
        var origin = new Position("r2k4/1K6/8/8/8/8/8/8 b - - 0 1");
        var move = new Move((int)BoardSquare.b7, (int)BoardSquare.a8, (int)Piece.K, isCapture: 1);

        Assert.NotNull(new Position(origin, move));
    }

    [TestCase(Constants.InitialPositionFEN, true)]
    [TestCase(Constants.EmptyBoardFEN, false)]
    [TestCase("K/8/8/8/8/8/8/8 w - - 0 1", false)]
    [TestCase("K/8/8/8/8/8/8/8 b - - 0 1", true)]
    [TestCase("k/8/8/8/8/8/8/8 w - - 0 1", true)]
    [TestCase("k/8/8/8/8/8/8/8 b - - 0 1", false)]
    [TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 0 1", false)]
    [TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1", true)]
    [TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR b KQkq - 0 1", false)]
    [TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1", true)]
    public void WasProduceByAValidMove(string fen, bool shouldBeValid)
    {
        Assert.AreEqual(shouldBeValid, new Position(fen).WasProduceByAValidMove());
    }

    [TestCase("7k/8/8/8/8/3B4/1K6/6Q1 b - - 0 1", 0)]
    [TestCase("7K/8/8/8/8/3b4/1k6/6q1 w - - 0 1", 0)]
    [TestCase("8/5K2/7p/6pk/6p1/6P1/7P/8 b - - 0 1", 0)]
    [TestCase("8/7p/6p1/6P1/6PK/5k1P/8/8 w - - 0 1", 0)]
    [TestCase("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", -EvaluationConstants.CheckMateEvaluation)]
    [TestCase("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -EvaluationConstants.CheckMateEvaluation)]
    public void EvaluateFinalPosition(string fen, int expectedEvaluationValue)
    {
        // Arrange
        var position = new Position(fen);
        Assert.IsEmpty(position.AllPossibleMoves().Where(move => new Position(position, move).IsValid()));

        // Act
        var noDepthResult = position.EvaluateFinalPosition(default, new(), default);
        var depthOneResult = position.EvaluateFinalPosition(1, new(), default);
        var depthTwoResult = position.EvaluateFinalPosition(2, new(), default);

        if (expectedEvaluationValue < 0)
        {
            Assert.AreEqual(expectedEvaluationValue, noDepthResult);

            Assert.True(noDepthResult < depthOneResult);
            Assert.True(depthOneResult < depthTwoResult);
        }
    }

    [Test]
    public void EvaluateFinalPosition_Threefold()
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

        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreEqual(0, eval);
    }

    [Test]
    public void StaticEvaluation_Threefold()
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
        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreEqual(0, eval);
    }

    [Test]
    public void StaticEvaluation_Threefold_CastleRightsRemoval()
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
        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreEqual(0, eval);

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
        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);

        eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreNotEqual(0, eval);

        repeatedMoves.TakeLast(4).ToList().ForEach(move => Assert.True(game.MakeMove(move)));
        Assert.AreEqual(repeatedMoves.Count + 4, game.MoveHistory.Count);
        eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreNotEqual(0, eval);

        repeatedMoves.TakeLast(4).ToList().ForEach(move => Assert.True(game.MakeMove(move)));
        Assert.AreEqual(repeatedMoves.Count + 8, game.MoveHistory.Count);
        eval = winningPosition.StaticEvaluation(game.PositionHashHistory, default);
        Assert.AreEqual(0, eval);
    }

    [Test]
    public void EvaluateFinalPosition_50MovesRule()
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

        Assert.AreEqual(51, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(new(), game.HalfMovesWithoutCaptureOrPawnMove);
        Assert.AreEqual(0, eval);
    }

    [Test]
    public void StaticEvaluation_50MovesRule()
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

        Assert.AreEqual(50, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(new(), game.HalfMovesWithoutCaptureOrPawnMove);
        Assert.AreEqual(0, eval);
    }

    [Test]
    public void StaticEvaluation_50MovesRule_Promotion()
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

        Assert.AreEqual(51, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(new(), game.HalfMovesWithoutCaptureOrPawnMove);
        Assert.AreNotEqual(0, eval);
    }

    [Test(Description = "Regression detected in v0.7.0")]
    public void StaticEvaluation_ShouldNotApply50MovesRule()
    {
        var game = PositionCommand.ParseGame("position startpos moves" +
            " g1f3 d7d5 d2d4 g8f6 g2g3 b8c6 f1g2 e7e6 b1c3 f8d6 e1g1 e8g8 f1e1 a8b8 e2e4 d5e4 c3e4 f6e4 e1e4 d6e7" +
            " b2b3 b7b5 c1b2 c8b7 e4e3 e7f6 f3e5 c6e7 g2b7 b8b7 e5g4 e7f5 g4f6 d8f6 e3e5 f5e7 d1f1 e7c6 f1g2 b7b6" +
            " e5e4 f8d8 c2c3 f6f5 g3g4 f5g6 a1e1 b6a6 a2a3 c6a5 g2g3 d8c8 g3d3 a6b6 d3d1 f7f5 e4e5 g6g4 d1g4 f5g4" +
            " e5e6 b6e6 e1e6 a5b3 e6e5 c7c5 e5e2 b3a5 d4c5 c8c5 e2e8 g8f7 e8a8 c5c7 a8b8 a7a6 g1g2 c7c6 b8h8 a5c4" +
            " b2c1 h7h6 h8a8 g7g5 a8a7 f7g6 a7a8 c6f6 a8g8 g6h5 g8e8 f6f3 c1e3 c4a3 e8d8 a3c2 e3d2 h5g6 g2g1 f3f6" +
            " d8g8 g6f5 g8d8 c2a3 d2e3 a3c4 e3d4 c4e5 g1g2 h6h5 h2h3 g4h3 g2h3 f6e6 h3g2 f5e4 d4e3 g5g4 d8h8 e6c6" +
            " e3d4 e5g6 h8e8 e4f5 g2g1 h5h4 g1h2 c6e6 e8a8 f5f4 d4e3 f4f3 h2g1 g4g3 a8a7 g6e7 a7a8 e7d5 a8f8 f3g4" +
            " f8g8 g4h3 g1f1 d5e3 f2e3 e6e3 g8g5 e3c3 f1e2 g3g2 e2f2 b5b4 g5g2 c3c2 f2e3 c2g2 e3d4 b4b3 d4d5 b3b2" +
            " d5e5 b2b1q e5f4 b1d3 f4e5 g2g6 e5f4 d3e2 f4f5 e2e6 f5f4 g6g4 f4f3 e6e8 f3f2 g4f4 f2g1 f4f6 g1h1 e8e6" +
            " h1g1 e6e5 g1h1 e5d5 h1g1 d5d4 g1h1 d4e4 h1g1 f6g6 g1f1 e4d3 f1e1 g6g1 e1f2 g1g2 f2e1 g2g6 e1f2 d3d2" +
            " f2f3 d2e1 f3f4 e1e2 f4f5 e2e6 f5f4 g6g4 f4f3 e6e8 f3f2 g4f4 f2g1 f4f6 g1h1 e8e6 h1g1 e6e5 g1h1 e5d5");

        var positionEval = game.CurrentPosition.StaticEvaluation(game.PositionHashHistory, game.HalfMovesWithoutCaptureOrPawnMove);
        Assert.NotZero(positionEval);
        Assert.Less(positionEval, -1500);
    }
}
