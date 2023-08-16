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
        Assert.AreEqual(fen, position.FEN());

        var newPosition = new Position(position);
        Assert.AreEqual(fen, newPosition.FEN());
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
        Assert.AreEqual(position.FEN(), clonedPosition.FEN());
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

    [TestCase(Constants.EmptyBoardFEN, false)]
    [TestCase("K/8/8/8/8/8/8/8 w - - 0 1", false)]
    [TestCase("K/8/8/8/8/8/8/8 b - - 0 1", false)]
    [TestCase("k/8/8/8/8/8/8/8 w - - 0 1", false)]
    [TestCase("k/8/8/8/8/8/8/8 b - - 0 1", false)]
    [TestCase(Constants.InitialPositionFEN, true)]
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

    [TestCase(Constants.EmptyBoardFEN, false, Ignore = "WasProduceByAValidMove doesn't check the presence of both kings on the board")]
    [TestCase("K/8/8/8/8/8/8/8 w - - 0 1", false, Ignore = "WasProduceByAValidMove doesn't check the presence of both kings on the board")]
    [TestCase("K/8/8/8/8/8/8/8 b - - 0 1", false, Ignore = "WasProduceByAValidMove doesn't check the presence of both kings on the board")]
    [TestCase("k/8/8/8/8/8/8/8 w - - 0 1", false, Ignore = "WasProduceByAValidMove doesn't check the presence of both kings on the board")]
    [TestCase("k/8/8/8/8/8/8/8 b - - 0 1", false, Ignore = "WasProduceByAValidMove doesn't check the presence of both kings on the board")]
    [TestCase(Constants.InitialPositionFEN, true)]
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
    [TestCase("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", -EvaluationConstants.CheckMateBaseEvaluation)]
    [TestCase("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -EvaluationConstants.CheckMateBaseEvaluation)]
    public void EvaluateFinalPosition(string fen, int expectedEvaluationValue)
    {
        // Arrange
        var position = new Position(fen);
        Assert.IsEmpty(position.AllPossibleMoves().Where(move => new Position(position, move).IsValid()));
        var isInCheck = position.IsInCheck();

        // Act
        var noDepthResult = Position.EvaluateFinalPosition(default, isInCheck);
        var depthOneResult = Position.EvaluateFinalPosition(1, isInCheck);
        var depthTwoResult = Position.EvaluateFinalPosition(2, isInCheck);

        if (expectedEvaluationValue < 0)
        {
            Assert.AreEqual(expectedEvaluationValue, noDepthResult);

            Assert.True(noDepthResult < depthOneResult);
            Assert.True(depthOneResult < depthTwoResult);
        }
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

        Assert.AreEqual(0, position.StaticEvaluation(default));
    }

    [TestCase("4k3/8/8/7Q/7q/8/8/4K3 w - - 0 1", "4k3/8/8/7Q/7q/8/4K3/8 w - - 0 1", Description = "King in 8th rank with queens > King in 7th rank with queens")]
    [TestCase("4k3/p7/8/8/8/8/P3K3/8 w - - 0 1", "4k3/p7/8/8/8/8/P7/4K3 w - - 0 1", Description = "King in 7th rank without queens > King in 8th rank without queens")]
    [TestCase("4k3/7p/8/8/4K3/8/7P/8 w - - 0 1", "4k3/7p/8/q7/4K3/Q7/7P/8 w - - 0 1", Description = "King in the center without queens > King in the center with queens")]
    public void StaticEvaluation_KingEndgame(string fen1, string fen2)
    {
        Assert.Greater(new Position(fen1).StaticEvaluation(default), new Position(fen2).StaticEvaluation(default));
    }

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . p p p . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P . P . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k3/1ppp4/8/8/8/8/PP1P4/4K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k4/4p1pp/8/8/8/8/4PPP1/3K4 b - - 0 1")]
    public void StaticEvaluation_IsolatedPawnPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-Configuration.EngineSettings.IsolatedPawnPenalty, evaluation);
    }

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   p p p . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   P . . . . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k3/ppp5/8/8/8/P7/PP6/4K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k4/6pp/7p/8/8/8/5PPP/3K4 b - - 0 1")]
    public void StaticEvaluation_DoublePawnPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-4 * Configuration.EngineSettings.DoubledPawnPenalty, evaluation);
    }

    /// <summary>
    /// Illegal position, but avoids any positional bonuses
    /// 8   . . . . k . . .
    /// 7   p p p . . p p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   P . . . . . . .
    /// 2   P P . . . . P P
    /// 1   P . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("7k/ppp2ppp/8/8/8/P7/PP4PP/P6K w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("k6p/pp4pp/7p/8/8/8/PPP2PPP/K7 b - - 0 1")]
    public void StaticEvaluation_TriplePawnPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-9 * Configuration.EngineSettings.DoubledPawnPenalty, evaluation);
    }

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . . . . . .
    /// 6   p . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P . P . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p7/8/8/8/PP1P4/6K1 w - - 0 1", BoardSquare.d2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k2p3/6pp/8/8/8/7P/6PP/1K6 b - - 0 1", BoardSquare.e7)]

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . . . . . .
    /// 6   p . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . P . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p7/8/8/3P4/PP6/6K1 w - - 0 1", BoardSquare.d3)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/6pp/4p3/8/8/7P/6PP/1K6 b - - 0 1", BoardSquare.e6)]

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . . . . . .
    /// 6   p . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . P . . . .
    /// 3   . . . . . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p7/8/3P4/8/PP6/6K1 w - - 0 1", BoardSquare.d4)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/6pp/8/4p3/8/7P/6PP/1K6 b - - 0 1", BoardSquare.e5)]

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . . . . . .
    /// 6   p . . . . . . .
    /// 5   . . . P . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p7/3P4/8/8/PP6/6K1 w - - 0 1", BoardSquare.d5)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/6pp/8/8/4p3/7P/6PP/1K6 b - - 0 1", BoardSquare.e4)]

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . . . . . .
    /// 6   p . . P . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p2P4/8/8/8/PP6/6K1 w - - 0 1", BoardSquare.d6)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/6pp/8/8/8/4p2P/6PP/1K6 b - - 0 1", BoardSquare.e3)]

    /// <summary>
    /// 8   . . . . . . k .
    /// 7   p p . P . . . .
    /// 6   p . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P . . . . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp1P4/p7/8/8/8/PP6/6K1 w - - 0 1", BoardSquare.d7)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/6pp/8/8/8/7P/4p1PP/1K6 b - - 0 1", BoardSquare.e2)]
    public void StaticEvaluation_PassedPawnBonus(string fen, BoardSquare square)
    {
        var position = new Position(fen);
        var rank = Constants.Rank[(int)square];
        if (position.Side == Side.Black)
        {
            rank = 7 - rank;
        }
        var piece = (int)(position.Side == Side.White ? Piece.P : Piece.p);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(
            4 * Configuration.EngineSettings.DoubledPawnPenalty
            - Configuration.EngineSettings.IsolatedPawnPenalty
            + EvaluationConstants.PositionalScore[piece][(int)square] * (position.Side == Side.White ? 1 : -1)
        + Configuration.EngineSettings.PassedPawnBonus[rank], evaluation);
    }

    /// <summary>
    /// 8   . . . . k . . r
    /// 7   p . . . . . . p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . P . . . . P
    /// 1   R . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k2r/p6p/8/8/8/8/2P4P/R3K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k3r/p4p2/8/8/8/8/P6P/R2K4 b - - 0 1")]
    public void StaticEvaluation_SemiOpenFileRookBonus(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);
        Assert.AreEqual(Configuration.EngineSettings.SemiOpenFileRookBonus, evaluation);
    }

    /// <summary>
    /// 8   . . . . k . . r
    /// 7   p . . . . . . p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . P . . . . P
    /// 1   . R . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k2r/p6p/8/8/8/8/2P4P/1R2K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k2r1/p4p2/8/8/8/8/P6P/R2K4 b - - 0 1")]
    public void StaticEvaluation_OpenFileRookBonus(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);
        Assert.AreEqual(Configuration.EngineSettings.OpenFileRookBonus, evaluation);
    }

    /// <summary>
    /// 8   . . . . k . . r
    /// 7   p . . . . . . r
    /// 6   . . . . . . . p
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   R . P . . . . P
    /// 1   R . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k2r/p6r/7p/8/8/8/R1P4P/R3K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k3r/p4p1r/8/8/8/P7/R6P/R2K4 b - - 0 1")]
    public void StaticEvaluation_DoubleSemiOpenFileRookBonus(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(2 * Configuration.EngineSettings.SemiOpenFileRookBonus, evaluation);
    }

    /// <summary>
    /// 8   . r . . k . . .
    /// 7   . r . . . . . p
    /// 6   p . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   P . . . . . . .
    /// 2   R . . . . . . P
    /// 1   R . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("1r2k3/1r5p/p7/8/8/P7/R6P/R3K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k3r/p6r/7p/8/8/7P/P5R1/3K2R1 b - - 0 1")]
    public void StaticEvaluation_DoubleOpenFileRookBonus(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-2 * Configuration.EngineSettings.OpenFileRookBonus, evaluation);
    }

    /// <summary>
    /// 8   . . . r . . k .
    /// 7   p p . p . . p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P . P P . . P P
    /// 1   . K . R . . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3r2k1/pp1p2pp/8/8/8/8/P1PP2PP/1K1R4 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4r1k1/pp2pp1p/8/8/8/8/PP2P1PP/1K2R3 b - - 0 1")]
    public void StaticEvaluation_SemiOpenFileKingPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-Configuration.EngineSettings.SemiOpenFileKingPenalty, evaluation);
    }

    /// <summary>
    /// 8   . . . r . . k .
    /// 7   p . p p . . p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P . P P . . P P
    /// 1   . K . R . . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3r2k1/p1pp2pp/8/8/8/8/P1PP2PP/1K1R4 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4r1k1/pp2pp1p/8/8/8/8/PP2PP1P/1K2R3 b - - 0 1")]
    public void StaticEvaluation_OpenFileKingPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(-Configuration.EngineSettings.OpenFileKingPenalty, evaluation);
    }

    /// <summary>
    /// No rooks = no bonus or penalty
    /// 8   . . . . . . k .
    /// 7   p . p . . . p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P . P . . . P P
    /// 1   . K . . . . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("6k1/p1p3pp/8/8/8/8/P1P3PP/1K6 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("6k1/pp3p1p/8/8/8/8/PP3P1P/1K6 b - - 0 1")]
    public void StaticEvaluation_NoOpenFileKingPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(0, evaluation);
    }

    /// <summary>
    /// No rooks = no bonus or penalty
    /// 8   . . . . . . k .
    /// 7   p . p . . . p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P . P . . . P P
    /// 1   . K . . . . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("6k1/pp1p2pp/8/8/8/8/P1PP2PP/1K6 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("6k1/pp2pp1p/8/8/8/8/PP2P1PP/1K6 b - - 0 1")]
    public void StaticEvaluation_NoSemiOpenFileKingPenalty(string fen)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(0, evaluation);
    }

    /// <summary>
    /// 8   . k . . . . . n
    /// 7   . . . . . p p p
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P P P . . . . .
    /// 1   . K . . . . . N
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="surroundingPieces"></param>
    [TestCase("1k5n/5ppp/8/8/8/8/PPP5/1K5N w - - 0 1", 3)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="surroundingPieces"></param>
    [TestCase("n5k1/5ppp/8/8/8/8/PPP5/N5K1 b - - 0 1", 3)]
    /// <summary>
    /// 8   . k . . . b . b
    /// 7   . . . . . n n n
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   N N N . . . . .
    /// 1   B K B . . . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="surroundingPieces"></param>
    [TestCase("1k3b1b/5nnn/8/8/8/8/NNN5/BKB5 w - - 0 1", 5)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="surroundingPieces"></param>
    [TestCase("5bkb/5nnn/8/8/8/8/NNN5/B1B3K1 b - - 0 1", 5)]
    public void StaticEvaluation_KingShieldBonus(string fen, int surroundingPieces)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(surroundingPieces * Configuration.EngineSettings.KingShieldBonus, evaluation);
    }

    /// <summary>
    /// 8   n . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . b . . . .
    /// 4   . . . B . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . N
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n3k3/1p6/8/3b4/3B4/8/6P1/4K2N w - - 0 1", 2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n2k4/1p6/8/4b3/4B3/8/6P1/3K3N b - - 0 1", 2)]
    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . .  p. . . . .
    /// 5   . . . b . . . .
    /// 4   . . . B . . . .
    /// 3   . . . . . P . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("4k3/1p6/2p5/3b4/3B4/5P2/6P1/4K3 w - - 0 1", 4)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("3k4/1p6/2p5/4b3/4B3/5P2/6P1/3K4 b - - 0 1", 4)]
    public void StaticEvaluation_BishopMobility(string fen, int mobilityDifference)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(mobilityDifference * Configuration.EngineSettings.BishopMobilityBonus, evaluation);
    }

    /// <summary>
    /// 8   n . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . q . . . .
    /// 4   . . . Q . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . N
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n3k3/1p6/8/3q4/3Q4/8/6P1/4K2N w - - 0 1", 2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n2k4/1p6/8/4q3/4Q3/8/6P1/3K3N b - - 0 1", 2)]
    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . . p . . . . .
    /// 5   . . . q . . . .
    /// 4   . . . Q . . . .
    /// 3   . . . . . P . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("4k3/1p6/2p5/3q4/3Q4/5P2/6P1/4K3 w - - 0 1", 4)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("3k4/1p6/2p5/4q3/4Q3/5P2/6P1/3K4 b - - 0 1", 4)]
    /// <summary>
    /// 8   n . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . q . . . .
    /// 4   . . . Q . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . N
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n3k3/1p6/8/3q4/3Q4/8/6P1/4K2N w - - 0 1", 2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n2k4/1p6/8/4q3/4Q3/8/6P1/3K3N b - - 0 1", 2)]
    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . p . . . . . .
    /// 6   . . p . . . . .
    /// 5   . . . q . . . .
    /// 4   . . . Q . . . .
    /// 3   . . . . . P . .
    /// 2   . . . . . . P .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("4k3/1p6/2p5/3q4/3Q4/5P2/6P1/4K3 w - - 0 1", 4)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("3k4/1p6/2p5/4q3/4Q3/5P2/6P1/3K4 b - - 0 1", 4)]
    /// <summary>
    /// 8   n . . . k . . .
    /// 7   . . . . . . . .
    /// 6   . . p . . . . .
    /// 5   . r . q . . . R
    /// 4   . . . Q . . . .
    /// 3   . . . . . P . .
    /// 2   . . . . . . . .
    /// 1   . . . . K . . N
    ///     a b c d e f g h
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n3k3/8/2p5/1r1q3R/3Q4/5P2/8/4K2N w - - 0 1", 5)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="mobilityDifference"></param>
    [TestCase("n2k4/8/2p5/4q3/r3Q1R1/5P2/8/3K3N b - - 0 1", 5)]
    public void StaticEvaluation_QueenMobility(string fen, int mobilityDifference)
    {
        var position = new Position(fen);
        var evaluation = position.StaticEvaluation(default);

        Assert.AreEqual(mobilityDifference * Configuration.EngineSettings.QueenMobilityBonus, evaluation);
    }
}
