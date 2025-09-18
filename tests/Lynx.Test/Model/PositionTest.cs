using Lynx.Model;
using NUnit.Framework;

using static Lynx.EvaluationConstants;
using static Lynx.Utils;

namespace Lynx.Test.Model;

public class PositionTest
{
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

	[TestCase(Constants.EmptyBoardFEN)]
	[TestCase("K/8/8/8/8/8/8/8 w - - 0 1")]
	[TestCase("K/8/8/8/8/8/8/8 b - - 0 1")]
	[TestCase("k/8/8/8/8/8/8/8 w - - 0 1")]
	[TestCase("k/8/8/8/8/8/8/8 b - - 0 1")]
	public void MissingKings(string fen)
	{
		Assert.Throws<LynxException>(() => new Position(fen));
	}

	[Explicit]
	[Category(Categories.LongRunning)]  // Can't run on debug due to position validation
	[TestCase(Constants.InitialPositionFEN, true)]
	[TestCase("r1k5/1K6/8/8/8/8/8/8 w - - 0 1", false)]
	[TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 0 1", false)]
	[TestCase("r1bqkbnr/pppp2pp/2n2p2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1", true)]
	[TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR b KQkq - 0 1", false)]
	[TestCase("r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1", true)]
	[TestCase("r1k5/1K6/8/8/8/8/8/8 w - - 0 1", false)]
	public void IsValid(string fen, bool shouldBeValid)
	{
		Assert.AreEqual(shouldBeValid, new Position(fen).IsValid());
	}

	[Explicit]
	[Category(Categories.LongRunning)]  // Can't run on debug due to position validation
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

	[Explicit]
	[Category(Categories.LongRunning)]  // Can't run on debug due to position validation
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
	[TestCase("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", -CheckMateBaseEvaluation)]
	[TestCase("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -CheckMateBaseEvaluation)]
	public void EvaluateFinalPosition(string fen, int expectedEvaluationValue)
	{
		// Arrange
		var position = new Position(fen);
		Assert.IsEmpty(MoveGenerator.GenerateAllMoves(position).Where(move =>
		{
			var newPosition = new Position(position);
			newPosition.MakeMove(move);
			return newPosition.IsValid();
		}));
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

    [TestCase("4k3/8/8/7Q/7q/8/4K3/8 w - - 0 1", "4k3/8/8/7Q/7q/8/8/4K3 w - - 0 1", Description = "King in 7th rank with queens > King in 8th rank with queens", IgnoreReason = "Can't understand PSQT any more")]
    [TestCase("4k3/p7/8/8/8/8/P3K3/8 w - - 0 1", "4k3/p7/8/8/8/8/P7/4K3 w - - 0 1", Description = "King in 7th rank without queens > King in 8th rank without queens", IgnoreReason = "Can't understand PSQT any more")]
    [TestCase("4k3/7p/8/8/4K3/8/7P/8 w - - 0 1", "4k3/7p/8/q7/4K3/Q7/7P/8 w - - 0 1", Description = "King in the center without queens > King in the center with queens", IgnoreReason = "Can't understand PSQT any more")]
    public void StaticEvaluation_KingEndgame(string fen1, string fen2)
    {
        Assert.Greater(new Position(fen1).StaticEvaluation().Score, new Position(fen2).StaticEvaluation().Score);
    }

    /// <summary>
    /// https://github.com/lynx-chess/Lynx/pull/510
    /// </summary>
    [TestCase("QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QPPPPPPP/K6k b - - 0 1", MinStaticEval, IgnoreReason = "Packed eval reduces max eval to a short, so over Short.MaxValue it overflows and produces unexpected results")]
    [TestCase("QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QPPPPPPP/K5k1 w - - 0 1", MaxStaticEval, IgnoreReason = "Packed eval reduces max eval to a short, so over Short.MaxValue it overflows and produces unexpected results")]
    [TestCase("8/QQQQQQ1/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQ6/K6k b - - 0 1", MinStaticEval, IgnoreReason = "It's just a pain to maintain this with bucketed PSQT tuning")]
    [TestCase("8/QQQQQQ1/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQ6/K5k1 w - - 0 1", MaxStaticEval, IgnoreReason = "It's just a pain to maintain this with bucketed PSQT tuning")]
    public void StaticEvaluation_Clamp(string fen, int expectedStaticEvaluation)
    {
        var position = new Position(fen);

		Assert.AreEqual(expectedStaticEvaluation, position.StaticEvaluation().Score);
	}

    [TestCase(0, 0)]
    [TestCase(0, 100)]
    [TestCase(100, 100)]
    [TestCase(100, 200)]
    public void TaperedEvaluation(int mg, int eg)
    {
        Assert.AreEqual(mg, Position.TaperedEvaluation(Pack((short)mg, (short)eg), 24));
        Assert.AreEqual(eg, Position.TaperedEvaluation(Pack((short)mg, (short)eg), 0));
    }
}
