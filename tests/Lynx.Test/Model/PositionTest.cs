using Lynx.Model;
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
            Assert.AreEqual(position.PieceBitBoards[piece], clonedPosition.PieceBitBoards[piece]);
        }

        for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
        {
            Assert.AreEqual(position.OccupancyBitBoards[occupancy], clonedPosition.OccupancyBitBoards[occupancy]);
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
            Assert.AreNotEqual(position.PieceBitBoards[piece], clonedPosition.PieceBitBoards[piece]);
        }

        for (int occupancy = 0; occupancy < position.OccupancyBitBoards.Length; ++occupancy)
        {
            Assert.AreNotEqual(position.OccupancyBitBoards[occupancy], clonedPosition.OccupancyBitBoards[occupancy]);
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
        var move = MoveExtensions.Encode((int)BoardSquare.b7, (int)BoardSquare.a8, (int)Piece.K, isCapture: 1);

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

    [TestCase("rnbqkbnr/ppp1pppp/3p4/1B6/8/4P3/PPPP1PPP/RNBQK1NR b KQkq - 1 2", true)]
    [TestCase("rnbqkbnr/ppp1pppp/3p4/1B6/8/4P3/PPPP1PPP/RNBQK1NR w KQkq - 1 2", false)]
    [TestCase("rnbqk1nr/pppp1ppp/4p3/8/1b6/3P1N2/PPP1PPPP/RNBQKB1R w KQkq - 2 3", true)]
    [TestCase("rnbqk1nr/pppp1ppp/4p3/8/1b6/3P1N2/PPP1PPPP/RNBQKB1R b KQkq - 2 3", false)]

    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR w KQkq - 1 3", true)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR b KQkq - 1 3", false)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR w KQkq - 1 3", true)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR b KQkq - 1 3", false)]
    public void IsInCheck(string fen, bool positionInCheck)
    {
        var position = new Position(fen);

        Assert.AreEqual(positionInCheck, position.IsInCheck());
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
        var isInCheck = position.IsInCheck();

        // Act
        var noDepthResult = Position.EvaluateFinalPosition(default, isInCheck, new(), default);
        var depthOneResult = Position.EvaluateFinalPosition(1, isInCheck, new(), default);
        var depthTwoResult = Position.EvaluateFinalPosition(2, isInCheck, new(), default);

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
        var winningPosition = new Position("7k/8/5KR1/8/8/8/5R2/8 w - - 0 1");

        var game = new Game(winningPosition);
        var repeatedMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),   // Triple position repetition
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.a2, (int)Piece.R),   // Other random move
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.h2, (int)Piece.R)    // Mate
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
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),
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
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)
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
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)
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
        var winningPosition = new Position("7k/8/5KR1/8/8/8/5R2/8 w - - 0 1");

        var game = new Game(winningPosition);
        var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k)
            };

        for (int i = 0; i < 98; ++i)
        {
            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

        Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[2]));
        Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[3]));
        Assert.True(game.MakeMove(MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.h2, (int)Piece.R)));   // Mate on move 51

        Assert.AreEqual(101, game.MoveHistory.Count);

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
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K)
            };

        for (int i = 0; i < 100; ++i)
        {
            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

        Assert.AreEqual(100, game.MoveHistory.Count);

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
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K)
            };

        for (int i = 0; i < 48; ++i)
        {
            Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

        Assert.True(game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)(Piece.q))));   // Promotion
        Assert.True(game.MakeMove(MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.c4, (int)Piece.K)));
        Assert.True(game.MakeMove(nonCaptureOrPawnMoveMoves[2]));

        Assert.AreEqual(51, game.MoveHistory.Count);

        var eval = winningPosition.StaticEvaluation(new(), game.HalfMovesWithoutCaptureOrPawnMove);
        Assert.AreNotEqual(0, eval);
    }

    [TestCase("k7/8/8/3B4/8/8/8/7K w - - 0 1", Description = "B")]
    [TestCase("k7/8/8/3b4/8/8/8/7K w - - 0 1", Description = "b")]
    [TestCase("k7/8/8/3N4/8/8/8/7K w - - 0 1", Description = "N")]
    [TestCase("k7/8/8/3N4/8/8/8/7K w - - 0 1", Description = "n")]
    [TestCase("k7/8/8/2NN4/8/8/8/7K w - - 0 1", Description = "N+N")]
    [TestCase("k7/8/8/2nn4/8/8/8/7K w - - 0 1", Description = "n+n")]
    [TestCase("k7/8/8/3B4/8/8/8/7K b - - 0 1", Description = "B")]
    [TestCase("k7/8/8/3b4/8/8/8/7K b - - 0 1", Description = "b")]
    [TestCase("k7/8/8/3N4/8/8/8/7K b - - 0 1", Description = "N")]
    [TestCase("k7/8/8/3N4/8/8/8/7K b - - 0 1", Description = "n")]
    [TestCase("k7/8/8/2NN4/8/8/8/7K b - - 0 1", Description = "N+N")]
    [TestCase("k7/8/8/2nn4/8/8/8/7K b - - 0 1", Description = "n+n")]
    public void StaticEvaluation_DrawDueToLackOfMaterial(string fen)
    {
        var position = new Position(fen);

        Assert.AreEqual(0, position.StaticEvaluation(new(), 0));
    }

    [TestCase("4k3/8/8/7Q/7q/8/8/4K3 w - - 0 1", "4k3/8/8/7Q/7q/8/4K3/8 w - - 0 1", Description = "King in 8th rank with queens > King in 7th rank with queens")]
    [TestCase("4k3/p7/8/8/8/8/P3K3/8 w - - 0 1", "4k3/p7/8/8/8/8/P7/4K3 w - - 0 1", Description = "King in 7th rank without queens > King in 8th rank without queens")]
    [TestCase("4k3/7p/8/8/4K3/8/7P/8 w - - 0 1", "4k3/7p/8/q7/4K3/Q7/7P/8 w - - 0 1", Description = "King in the center without queens > King in the center with queens")]
    public void StaticEvaluation_KingEndgame(string fen1, string fen2)
    {
        Assert.Greater(new Position(fen1).StaticEvaluation(new(), default), new Position(fen2).StaticEvaluation(new(), default));
    }
}
