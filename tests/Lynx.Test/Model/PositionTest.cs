using Lynx.Model;
using NUnit.Framework;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
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
        Assert.IsEmpty(position.GenerateAllMoves().Where(move =>
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

    [TestCase("8/k7/8/3B4/8/8/8/7K w - - 0 1", Description = "B")]
    [TestCase("8/k7/8/3b4/8/8/8/7K w - - 0 1", Description = "b")]
    [TestCase("k7/8/8/3N4/8/8/8/7K w - - 0 1", Description = "N")]
    [TestCase("k7/8/8/3N4/8/8/8/7K w - - 0 1", Description = "n")]
    [TestCase("k7/8/8/2NN4/8/8/8/7K w - - 0 1", Description = "N+N")]
    [TestCase("k7/8/8/2nn4/8/8/8/7K w - - 0 1", Description = "n+n")]
    [TestCase("k7/8/8/3B4/8/8/8/6K1 b - - 0 1", Description = "B")]
    [TestCase("k7/8/8/3b4/8/8/8/6K1 b - - 0 1", Description = "b")]
    [TestCase("k7/8/8/3N4/8/8/8/7K b - - 0 1", Description = "N")]
    [TestCase("k7/8/8/3N4/8/8/8/7K b - - 0 1", Description = "n")]
    [TestCase("k7/8/8/2NN4/8/8/8/7K b - - 0 1", Description = "N+N")]
    [TestCase("k7/8/8/2nn4/8/8/8/7K b - - 0 1", Description = "n+n")]
    public void StaticEvaluation_DrawDueToLackOfMaterial(string fen)
    {
        var position = new Position(fen);

        Assert.AreEqual(0, position.StaticEvaluation().Score);
    }

    [TestCase("4k3/8/8/7Q/7q/8/4K3/8 w - - 0 1", "4k3/8/8/7Q/7q/8/8/4K3 w - - 0 1", Description = "King in 7th rank with queens > King in 8th rank with queens", IgnoreReason = "Can't understand PSQT any more")]
    [TestCase("4k3/p7/8/8/8/8/P3K3/8 w - - 0 1", "4k3/p7/8/8/8/8/P7/4K3 w - - 0 1", Description = "King in 7th rank without queens > King in 8th rank without queens", IgnoreReason = "Can't understand PSQT any more")]
    [TestCase("4k3/7p/8/8/4K3/8/7P/8 w - - 0 1", "4k3/7p/8/q7/4K3/Q7/7P/8 w - - 0 1", Description = "King in the center without queens > King in the center with queens", IgnoreReason = "Can't understand PSQT any more")]
    public void StaticEvaluation_KingEndgame(string fen1, string fen2)
    {
        Assert.Greater(new Position(fen1).StaticEvaluation().Score, new Position(fen2).StaticEvaluation().Score);
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
    [TestCase("7k/1ppp2pp/8/8/8/8/PP1P2PP/7K w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("3k4/3p2pp/8/8/8/8/4PPP1/3K4 b - - 0 1")]
    public void StaticEvaluation_IsolatedPawnPenalty(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.P)
            - AdditionalPieceEvaluation(position, Piece.p);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        var expectedEval = UnpackMG(IsolatedPawnPenalty[Constants.File[(int)BoardSquare.d3]]) - UnpackMG(PawnPhalanxBonus[1]);

        Assert.AreEqual(expectedEval, evaluation);
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
    //[TestCase("4k3/ppp5/8/8/8/P7/PP6/4K3 w - - 0 1")]
    ///// <summary>
    ///// Previous one mirrored
    ///// </summary>
    ///// <param name="fen"></param>
    //[TestCase("3k4/6pp/7p/8/8/8/5PPP/3K4 b - - 0 1")]
    //public void StaticEvaluation_DoublePawnPenalty(string fen)
    //{
    //    Position position = new Position(fen);
    //    int evaluation = AdditionalPieceEvaluation(position, Piece.P)
    //        - AdditionalPieceEvaluation(position, Piece.p);

    //    if (position.Side == Side.Black)
    //    {
    //        evaluation = -evaluation;
    //    }

    //    Assert.AreEqual(4 * Configuration.EngineSettings.DoubledPawnPenalty.MG, evaluation);
    //}

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
    //[TestCase("7k/ppp2ppp/8/8/8/P7/PP4PP/P6K w - - 0 1")]
    ///// <summary>
    ///// Previous one mirrored
    ///// </summary>
    ///// <param name="fen"></param>
    //[TestCase("k6p/pp4pp/7p/8/8/8/PPP2PPP/K7 b - - 0 1")]
    //public void StaticEvaluation_TriplePawnPenalty(string fen)
    //{
    //    Position position = new Position(fen);
    //    int evaluation = AdditionalPieceEvaluation(position, Piece.P)
    //        - AdditionalPieceEvaluation(position, Piece.p);

    //    if (position.Side == Side.Black)
    //    {
    //        evaluation = -evaluation;
    //    }

    //    Assert.AreEqual(9 * Configuration.EngineSettings.DoubledPawnPenalty.MG, evaluation);
    //}

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
    [TestCase("6k1/pp6/p7/8/8/8/PP1P4/6K1 w - - 0 1", BoardSquare.d2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/3p2pp/8/8/8/7P/6PP/1K6 b - - 0 1", BoardSquare.d7)]

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
    [TestCase("6k1/pp6/p7/8/8/3P4/PP6/6K1 w - - 0 1", BoardSquare.d3)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/6pp/3p4/8/8/7P/6PP/1K6 b - - 0 1", BoardSquare.d6)]

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
    [TestCase("6k1/pp6/p7/8/3P4/8/PP6/6K1 w - - 0 1", BoardSquare.d4)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/6pp/8/3p4/8/7P/6PP/1K6 b - - 0 1", BoardSquare.d5)]

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
    [TestCase("6k1/pp6/p7/3P4/8/8/PP6/6K1 w - - 0 1", BoardSquare.d5)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/6pp/8/8/3p4/7P/6PP/1K6 b - - 0 1", BoardSquare.d4)]

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
    [TestCase("6k1/pp6/p2P4/8/8/8/PP6/6K1 w - - 0 1", BoardSquare.d6)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/6pp/8/8/8/3p3P/6PP/1K6 b - - 0 1", BoardSquare.d3)]

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
    [TestCase("6k1/pp1P4/p7/8/8/8/PP6/6K1 w - - 0 1", BoardSquare.d7)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("1k6/6pp/8/8/8/7P/3p2PP/1K6 b - - 0 1", BoardSquare.d2)]
    public void StaticEvaluation_PassedPawnBonus(string fen, BoardSquare square)
    {
        var position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.P)
            - AdditionalPieceEvaluation(position, Piece.p);

        var rank = Constants.Rank[(int)square];
        var passedPawnsMask = Masks.WhitePassedPawnMasks[(int)square];

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
            rank = 7 - rank;
            passedPawnsMask = Masks.BlackPassedPawnMasks[(int)square];
        }

        var whiteKingDistance = Constants.ChebyshevDistance[(int)square][position.PieceBitBoards[(int)Piece.K].GetLS1BIndex()];
        var blackKingDistance = Constants.ChebyshevDistance[(int)square][position.PieceBitBoards[(int)Piece.k].GetLS1BIndex()];

        var friendlyKingDistance = position.Side == Side.White
            ? whiteKingDistance
            : blackKingDistance;

        var enemyKingDistance = position.Side == Side.White
            ? blackKingDistance
            : whiteKingDistance;

        var expectedEval = 0;
        if ((passedPawnsMask & position.OccupancyBitBoards[OppositeSide(position.Side)]) == 0)
        {
            expectedEval += UnpackMG(PassedPawnBonusNoEnemiesAheadBonus[0][rank]);
            expectedEval += UnpackMG(PassedPawnBonusNoEnemiesAheadEnemyBonus[0][rank]);
        }

        Assert.AreEqual(
            expectedEval
            //(-4 * Configuration.EngineSettings.DoubledPawnPenalty.MG)
            + UnpackMG(IsolatedPawnPenalty[Constants.File[(int)square]])
            + UnpackMG(PassedPawnBonus[0][rank])
            + UnpackMG(PassedPawnEnemyBonus[0][rank])
            + UnpackMG(FriendlyKingDistanceToPassedPawnBonus[friendlyKingDistance])
            + UnpackMG(EnemyKingDistanceToPassedPawnPenalty[enemyKingDistance]),

            evaluation);
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
    [TestCase("4k2r/p6p/8/8/8/8/2P4P/R3K3 w - - 0 1", 9, 2, 0)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("3k3r/p4p2/8/8/8/8/P6P/R2K4 b - - 0 1", 9, 2, 7)]
    public void StaticEvaluation_SemiOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove, int semiopenFile)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(
            UnpackMG(SemiOpenFileRookBonus[0][semiopenFile])
                + UnpackMG(SemiOpenFileRookEnemyBonus[0][semiopenFile])
                + UnpackMG(RookMobilityBonus[rookMobilitySideToMove]) - UnpackMG(RookMobilityBonus[rookMobilitySideNotToMove]),
            evaluation);
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
    [TestCase("7r/2p1k2p/8/8/8/8/2P1K2P/1R6 w - - 0 1", 13, 7, 1)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("6r1/p2k1p2/8/8/8/8/P2K1P2/R7 b - - 0 1", 13, 7, 6)]
    public void StaticEvaluation_OpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove, int openFile)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(
            UnpackMG(OpenFileRookBonus[0][openFile])
            + UnpackMG(OpenFileRookEnemyBonus[0][openFile])
            + UnpackMG(RookMobilityBonus[rookMobilitySideToMove]) - UnpackMG(RookMobilityBonus[rookMobilitySideNotToMove]),
            evaluation);
    }

    /// <summary>
    /// 8   . . . . k. .r
    /// 7   p. . . . . .r
    /// 6   . . . . . . . p
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   R. .P. . .P
    /// 1   R. .K. . . .
    ///     a b c d e f g h
    /// </summary>
    [TestCase("4k2r/p6r/7p/8/8/8/R2P3P/R2K4 w - - 0 1", 7, 6, 0)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("4k2r/p3p2r/8/8/8/P7/R6P/R2K4 b - - 0 1", 7, 6, 7)]
    public void StaticEvaluation_DoubleSemiOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove, int semiopenFile)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(
            (2 * UnpackMG(SemiOpenFileRookBonus[0][semiopenFile]))
            + (2 * UnpackMG(SemiOpenFileRookEnemyBonus[0][semiopenFile]))
            + UnpackMG(RookMobilityBonus[rookMobilitySideToMove]) - UnpackMG(RookMobilityBonus[rookMobilitySideNotToMove]),
        evaluation);
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
    [TestCase("1r5k/1r5p/2p5/8/8/2P5/2R4P/2R4K w - - 0 1", 6, 11, 1)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("k4r2/p4r2/5p2/8/8/5P2/P5R1/K5R1 b - - 0 1", 6, 11, 6)]
    public void StaticEvaluation_DoubleOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove, int openFile)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(
            (-2 * UnpackMG(OpenFileRookBonus[0][openFile]))
            + (-2 * UnpackMG(OpenFileRookEnemyBonus[0][openFile]))
            + UnpackMG(RookMobilityBonus[rookMobilitySideToMove])
            - UnpackMG(RookMobilityBonus[rookMobilitySideNotToMove]),
            evaluation);
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
    [TestCase("6k1/p1p3pp/8/8/8/8/P1P3PP/1K6 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("6k1/pp3p1p/8/8/8/8/PP3P1P/1K6 b - - 0 1")]
    [Ignore("Broken by virtual king mobility")]
    public void StaticEvaluation_NoOpenFileKingPenalty(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalKingEvaluation(position, Piece.K)
            - AdditionalKingEvaluation(position, Piece.k);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(0, evaluation);
    }

#pragma warning disable S4144 // Methods should not have identical implementations
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
    [TestCase("6k1/pp1p2pp/8/8/8/8/P1PP2PP/1K6 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("6k1/pp2pp1p/8/8/8/8/PP2P1PP/1K6 b - - 0 1")]
    public void StaticEvaluation_NoSemiOpenFileKingPenalty(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalKingEvaluation(position, Piece.K)
            - AdditionalKingEvaluation(position, Piece.k);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(0, evaluation);
    }
#pragma warning restore S4144 // Methods should not have identical implementations

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
    [TestCase("1k5n/5ppp/8/8/8/8/PPP5/1K5N w - - 0 1", 3)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
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
    [TestCase("1k3b1b/5nnn/8/8/8/8/NNN5/BKB5 w - - 0 1", 5)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("5bkb/5nnn/8/8/8/8/NNN5/B1B3K1 b - - 0 1", 5)]
    [Ignore("Broken by virtual king mobility")]
    public void StaticEvaluation_KingShieldBonus(string fen, int surroundingPieces)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalKingEvaluation(position, Piece.K)
            - AdditionalKingEvaluation(position, Piece.k);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(surroundingPieces * UnpackEG(KingShieldBonus), evaluation);
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
    [TestCase("n3k3/1n6/8/3b4/3B4/8/6N1/4K2N w - - 0 1", 13, 10)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("n2k4/1n6/8/4b3/4B3/8/6N1/3K3N b - - 0 1", 13, 10)]
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
    [TestCase("4k3/1n6/2n5/3b4/3B4/5N2/6N1/4K3 w - - 0 1", 13, 8)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("3k4/1n6/2n5/4b3/4B3/5N2/6N1/3K4 b - - 0 1", 13, 8)]
    public void StaticEvaluation_BishopMobility(string fen, int sideToMoveMobilityCount, int nonSideToMoveMobilityCount)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.B)
            - AdditionalPieceEvaluation(position, Piece.b);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(UnpackMG(BishopMobilityBonus[sideToMoveMobilityCount]) - UnpackMG(BishopMobilityBonus[nonSideToMoveMobilityCount]), evaluation);
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
    [TestCase("n7/1p6/3k4/3q4/3Q4/3K4/6P1/7N w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("n7/1p6/4k3/4q3/4Q3/4K3/6P1/7N b - - 0 1")]
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
    [TestCase("4k3/1p6/2p5/3q4/3Q4/5P2/6P1/4K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("3k4/1p6/2p5/4q3/4Q3/5P2/6P1/3K4 b - - 0 1")]
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
    [TestCase("n7/1p6/3k4/3q4/3Q4/3K4/6P1/7N w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("n7/1p6/4k3/4q3/4Q3/4K3/6P1/7N b - - 0 1")]
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
    [TestCase("4k3/1p6/2p5/3q4/3Q4/5P2/6P1/4K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("3k4/1p6/2p5/4q3/4Q3/5P2/6P1/3K4 b - - 0 1")]
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
    [TestCase("2n1kn2/4n3/2p5/1r1q3R/3Q4/5P2/6NN/7K w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    [TestCase("n7/k7/2p5/4q3/r3Q1R1/5P2/8/3K3N b - - 0 1")]
    public void StaticEvaluation_QueenMobility(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.Q)
            - AdditionalPieceEvaluation(position, Piece.q);

        BitBoard whitePawnAttacks = position.PieceBitBoards[(int)Piece.P].ShiftUpRight() | position.PieceBitBoards[(int)Piece.P].ShiftUpLeft();
        BitBoard blackPawnAttacks = position.PieceBitBoards[(int)Piece.p].ShiftDownRight() | position.PieceBitBoards[(int)Piece.p].ShiftDownLeft();

        var whiteMobility =
            (Attacks.QueenAttacks(position.PieceBitBoards[(int)Piece.Q].GetLS1BIndex(), position.OccupancyBitBoards[(int)Side.Both])
                & (~(position.OccupancyBitBoards[(int)Side.White] | blackPawnAttacks)))
            .CountBits();

        var blackMobility =
            (Attacks.QueenAttacks(position.PieceBitBoards[(int)Piece.q].GetLS1BIndex(), position.OccupancyBitBoards[(int)Side.Both])
                & (~(position.OccupancyBitBoards[(int)Side.Black] | whitePawnAttacks)))
            .CountBits();

        var expectedEvaluation = QueenMobilityBonus[whiteMobility] - QueenMobilityBonus[blackMobility];

        Assert.AreEqual(UnpackMG(expectedEvaluation), evaluation);
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

    [TestCase("k7/8/8/3K4/8/8/8/8 w - - 0 1", true, "K vs k")]
    [TestCase("8/8/8/8/3k4/8/8/K7 w - - 0 1", true, "K vs k")]
    public void StaticEvaluation_PawnlessEndgames_KingVsKing(string fen, bool isDrawExpected, string _)
    {
        EvaluateDrawOrNotDraw(fen, isDrawExpected, 0);
    }

    [TestCase("1k6/8/8/8/8/8/1B6/1K6 w - - 0 1", true, 1, "B")]
    [TestCase("1k6/1b6/8/8/8/8/8/1K6 w - - 0 1", true, 1, "b")]
    [TestCase("1k6/8/8/8/8/8/1N6/1K6 w - - 0 1", true, 1, "N")]
    [TestCase("1k6/1n6/8/8/8/8/8/1K6 w - - 0 1", true, 1, "n")]
    [TestCase("2k5/8/8/8/8/8/1R6/1K6 w - - 0 1", false, 2, "R")]
    [TestCase("rk6/8/8/8/8/8/8/1K6 w - - 0 1", false, 2, "r")]
    [TestCase("2k5/8/8/8/8/8/1Q6/1K6 w - - 0 1", false, 4, "Q")]
    [TestCase("qk6/8/8/8/8/8/8/1K6 w - - 0 1", false, 4, "q")]
    public void StaticEvaluation_PawnlessEndgames_SinglePiece(string fen, bool isDrawExpected, int expectedPhase, string _)
    {
        EvaluateDrawOrNotDraw(fen, isDrawExpected, expectedPhase);
    }

    [TestCase("1k6/n7/8/8/8/8/N7/1K6 w - - 0 1", true, "N vs n")]
    [TestCase("1k6/b7/8/8/8/8/B7/1K6 w - - 0 1", true, "B vs b")]
    [TestCase("1k6/n7/8/8/8/8/B7/1K6 w - - 0 1", true, "B vs n")]
    [TestCase("1k6/b7/8/8/8/8/N7/1K6 w - - 0 1", true, "N vs b")]
    [TestCase("1k6/8/8/8/8/8/NB6/1K6 w - - 0 1", false, "BN")]
    [TestCase("1k6/8/8/8/8/8/BB6/1K6 w - - 0 1", false, "BB")]
    [TestCase("1k6/bb6/8/8/8/8/8/1K6 w - - 0 1", false, "bb")]
    [TestCase("1k6/bn6/8/8/8/8/8/1K6 w - - 0 1", false, "bn")]
    [TestCase("1k6/8/8/8/8/8/NN6/1K6 w - - 0 1", true, "NN")]
    [TestCase("1k6/nn6/8/8/8/8/8/1K6 w - - 0 1", true, "nn")]
    public void StaticEvaluation_PawnlessEndgames_TwoMinorPieces(string fen, bool isDrawExpected, string _)
    {
        EvaluateDrawOrNotDraw(fen, isDrawExpected, 2);
    }

    [TestCase("8/8/3kb3/8/8/2NN4/3K4/8 w - - 0 1", true, "NN vs b")]
    [TestCase("8/8/3knn2/8/8/3B4/3K4/8 w - - 0 1", true, "B vs nn")]
    [TestCase("8/8/3kn3/8/8/2NN4/3K4/8 w - - 0 1", true, "NN vs n")]
    [TestCase("8/8/3knn2/8/8/3N4/3K4/8 w - - 0 1", true, "N vs nn")]
    public void StaticEvaluation_PawnlessEndgames_TwoMinorPiecesVsOne(string fen, bool isDrawExpected, string _)
    {
        EvaluateDrawOrNotDraw(fen, isDrawExpected, 3);
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

    [Test]
    public void ScaleEvalWith50MovesDrawDistance()
    {
        const string queenVsRookPosition = "8/4k3/4r3/3Q4/3K4/8/8/8 w - - 0 1";

        var position = new Position(queenVsRookPosition);

        Assert.Greater(position.StaticEvaluation(0), position.StaticEvaluation(10));
        Assert.AreEqual((int)(0.5 * position.StaticEvaluation(0).Score), position.StaticEvaluation(100).Score);
        Assert.AreEqual((int)(0.75 * position.StaticEvaluation(0).Score), position.StaticEvaluation(50).Score);
    }

    [Test]
    public void FreeAndNonAttackedSquares()
    {
        /// <summary>
        /// 8   0 0 0 0 0 0 0 0
        /// 7   0 0 0 0 0 0 0 0
        /// 6   0 0 0 0 0 0 0 0
        /// 5   0 0 0 0 0 0 0 0
        /// 4   0 0 0 0 0 0 0 0
        /// 3   0 0 0 0 0 0 0 0
        /// 2   0 0 0 0 0 0 0 0
        /// 1   0 0 0 0 0 1 1 0
        ///     a b c d e f g h
        /// </summary>
        const BitBoard WhiteShortCastleFreeSquares = 0x6000000000000000;

        /// <summary>
        /// 8   0 0 0 0 0 0 0 0
        /// 7   0 0 0 0 0 0 0 0
        /// 6   0 0 0 0 0 0 0 0
        /// 5   0 0 0 0 0 0 0 0
        /// 4   0 0 0 0 0 0 0 0
        /// 3   0 0 0 0 0 0 0 0
        /// 2   0 0 0 0 0 0 0 0
        /// 1   0 1 1 1 0 0 0 0
        ///     a b c d e f g h
        /// </summary>
        const BitBoard WhiteLongCastleFreeSquares = 0xe00000000000000;

        /// <summary>
        /// 8   0 0 0 0 0 1 1 0
        /// 7   0 0 0 0 0 0 0 0
        /// 6   0 0 0 0 0 0 0 0
        /// 5   0 0 0 0 0 0 0 0
        /// 4   0 0 0 0 0 0 0 0
        /// 3   0 0 0 0 0 0 0 0
        /// 2   0 0 0 0 0 0 0 0
        /// 1   0 0 0 0 0 0 0 0
        ///     a b c d e f g h
        /// </summary>
        const BitBoard BlackShortCastleFreeSquares = 0x60;

        /// <summary>
        /// 8   0 1 1 1 0 0 0 0
        /// 7   0 0 0 0 0 0 0 0
        /// 6   0 0 0 0 0 0 0 0
        /// 5   0 0 0 0 0 0 0 0
        /// 4   0 0 0 0 0 0 0 0
        /// 3   0 0 0 0 0 0 0 0
        /// 2   0 0 0 0 0 0 0 0
        /// 1   0 0 0 0 0 0 0 0
        ///     a b c d e f g h
        /// </summary>
        const BitBoard BlackLongCastleFreeSquares = 0xe;

        var position = new Position(Constants.InitialPositionFEN);

        Assert.AreEqual(WhiteShortCastleFreeSquares, position.KingsideCastlingFreeSquares[(int)Side.White]);
        Assert.AreEqual(BlackShortCastleFreeSquares, position.KingsideCastlingFreeSquares[(int)Side.Black]);
        Assert.AreEqual(WhiteLongCastleFreeSquares, position.QueensideCastlingFreeSquares[(int)Side.White]);
        Assert.AreEqual(BlackLongCastleFreeSquares, position.QueensideCastlingFreeSquares[(int)Side.Black]);

        var nonAttackedWhiteShortCastleSquares = position.KingsideCastlingFreeSquares[(int)Side.White];
        nonAttackedWhiteShortCastleSquares.SetBit(Constants.InitialWhiteKingSquare);
        nonAttackedWhiteShortCastleSquares.SetBit(Constants.WhiteKingShortCastleSquare);

        var nonAttackedWhiteLongCastleSquares = position.QueensideCastlingFreeSquares[(int)Side.White];
        nonAttackedWhiteLongCastleSquares.PopBit(BoardSquare.b1);
        nonAttackedWhiteLongCastleSquares.SetBit(Constants.InitialWhiteKingSquare);
        nonAttackedWhiteLongCastleSquares.SetBit(Constants.WhiteKingLongCastleSquare);

        var nonAttackedBlackShortCastleSquares = position.KingsideCastlingFreeSquares[(int)Side.Black];
        nonAttackedBlackShortCastleSquares.SetBit(Constants.InitialBlackKingSquare);
        nonAttackedBlackShortCastleSquares.SetBit(Constants.BlackKingShortCastleSquare);

        var nonAttackedBlackLongCastleSquares = position.QueensideCastlingFreeSquares[(int)Side.Black];
        nonAttackedBlackLongCastleSquares.PopBit(BoardSquare.b8);
        nonAttackedBlackLongCastleSquares.SetBit(Constants.InitialBlackKingSquare);
        nonAttackedBlackLongCastleSquares.SetBit(Constants.BlackKingLongCastleSquare);

        Assert.AreEqual(nonAttackedWhiteShortCastleSquares, position.KingsideCastlingNonAttackedSquares[(int)Side.White]);
        Assert.AreEqual(nonAttackedWhiteLongCastleSquares, position.QueensideCastlingNonAttackedSquares[(int)Side.White]);
        Assert.AreEqual(nonAttackedBlackShortCastleSquares, position.KingsideCastlingNonAttackedSquares[(int)Side.Black]);
        Assert.AreEqual(nonAttackedBlackLongCastleSquares, position.QueensideCastlingNonAttackedSquares[(int)Side.Black]);
    }

    private static int AdditionalPieceEvaluation(Position position, Piece piece)
    {
        var whiteKing = position.PieceBitBoards[(int)Piece.K].GetLS1BIndex();
        var blackKing = position.PieceBitBoards[(int)Piece.k].GetLS1BIndex();

        var sameSideKingSquare = piece <= Piece.K
            ? whiteKing
            : blackKing;

        var oppositeSideKingSquare = piece <= Piece.K
            ? blackKing
            : whiteKing;

        BitBoard whitePawnAttacks = position.PieceBitBoards[(int)Piece.P].ShiftUpRight() | position.PieceBitBoards[(int)Piece.P].ShiftUpLeft();
        BitBoard blackPawnAttacks = position.PieceBitBoards[(int)Piece.p].ShiftDownRight() | position.PieceBitBoards[(int)Piece.p].ShiftDownLeft();

        var oppositeSidePawnAttacks = piece <= Piece.K
            ? blackPawnAttacks
            : whitePawnAttacks;

        var pieceSide = (int)piece <= (int)Piece.K
            ? (int)Side.White
            : (int)Side.Black;

        var bitBoard = position.PieceBitBoards[(int)piece];
        int eval = 0;

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        while (!bitBoard.Empty())
        {
            var pieceSquareIndex = bitBoard.GetLS1BIndex();
            bitBoard.ResetLS1B();
            eval += UnpackMG(position.AdditionalPieceEvaluation(ref evaluationContext, 0, 0, pieceSquareIndex, (int)piece, pieceSide, sameSideKingSquare, oppositeSideKingSquare, oppositeSidePawnAttacks));
        }

        return eval;
    }

    private static int AdditionalKingEvaluation(Position position, Piece piece)
    {
        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                bitboard.ResetLS1B();
            }
        }

        BitBoard whitePawnAttacks = position.PieceBitBoards[(int)Piece.P].ShiftUpRight() | position.PieceBitBoards[(int)Piece.P].ShiftUpLeft();
        BitBoard blackPawnAttacks = position.PieceBitBoards[(int)Piece.p].ShiftDownRight() | position.PieceBitBoards[(int)Piece.p].ShiftDownLeft();

        var bitBoard = position.PieceBitBoards[(int)piece].GetLS1BIndex();

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        return UnpackEG(piece == Piece.K
            ? position.KingAdditionalEvaluation(ref evaluationContext, bitBoard, 0, (int)Side.White, blackPawnAttacks)
            : position.KingAdditionalEvaluation(ref evaluationContext, bitBoard, 0, (int)Side.Black, whitePawnAttacks));
    }

    private static void EvaluateDrawOrNotDraw(string fen, bool isDrawExpected, int expectedPhase)
    {
        var position = new Position(fen);
        var (score, phase) = position.StaticEvaluation();

        Assert.AreEqual(expectedPhase, phase);

        if (isDrawExpected)
        {
            Assert.AreEqual(0, score);
        }
        else
        {
            Assert.AreNotEqual(0, score);
        }
    }
}
