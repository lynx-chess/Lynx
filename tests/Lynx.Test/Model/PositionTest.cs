﻿using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

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
        var move = MoveExtensions.EncodeCapture((int)BoardSquare.b7, (int)BoardSquare.a8, (int)Piece.K, capturedPiece: 1);

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
    [TestCase("7k/8/8/8/8/8/1K5R/6R1 b - - 0 1", -CheckMateBaseEvaluation)]
    [TestCase("7K/8/8/8/8/8/1k5r/6r1 w - - 0 1", -CheckMateBaseEvaluation)]
    public void EvaluateFinalPosition(string fen, int expectedEvaluationValue)
    {
        // Arrange
        var position = new Position(fen);
        Assert.IsEmpty(MoveGenerator.GenerateAllMoves(position).Where(move => new Position(position, move).IsValid()));
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

        Assert.AreEqual(0, position.StaticEvaluation().Score);
    }

    [TestCase("4k3/8/8/7Q/7q/8/4K3/8 w - - 0 1", "4k3/8/8/7Q/7q/8/8/4K3 w - - 0 1", Description = "King in 7th rank with queens > King in 8th rank with queens", IgnoreReason = "Can't understand PSQT any more")]
    [TestCase("4k3/p7/8/8/8/8/P3K3/8 w - - 0 1", "4k3/p7/8/8/8/8/P7/4K3 w - - 0 1", Description = "King in 7th rank without queens > King in 8th rank without queens")]
    [TestCase("4k3/7p/8/8/4K3/8/7P/8 w - - 0 1", "4k3/7p/8/q7/4K3/Q7/7P/8 w - - 0 1", Description = "King in the center without queens > King in the center with queens")]
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
    /// <param name="fen"></param>
    [TestCase("4k3/1ppp4/8/8/8/8/PP1P4/4K3 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k4/4p1pp/8/8/8/8/4PPP1/3K4 b - - 0 1")]
    public void StaticEvaluation_IsolatedPawnPenalty(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.P)
            - AdditionalPieceEvaluation(position, Piece.p);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(IsolatedPawnPenalty.MG, evaluation);
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
    /// <param name="fen"></param>
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
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("6k1/pp6/p7/8/8/8/PP1P4/6K1 w - - 0 1", BoardSquare.d2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="square"></param>
    [TestCase("1k6/4p1pp/8/8/8/7P/6PP/1K6 b - - 0 1", BoardSquare.e7)]

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
        int evaluation = AdditionalPieceEvaluation(position, Piece.P)
            - AdditionalPieceEvaluation(position, Piece.p);

        var rank = Constants.Rank[(int)square];
        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
            rank = 7 - rank;
        }

        Assert.AreEqual(
            //(-4 * Configuration.EngineSettings.DoubledPawnPenalty.MG)
            +IsolatedPawnPenalty.MG
            + PassedPawnBonus[rank].MG,

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
    /// <param name="fen"></param>
    [TestCase("4k2r/p6p/8/8/8/8/2P4P/R3K3 w - - 0 1", 9, 2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k3r/p4p2/8/8/8/8/P6P/R2K4 b - - 0 1", 9, 2)]
    public void StaticEvaluation_SemiOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(SemiOpenFileRookBonus.MG
                + RookMobilityBonus[rookMobilitySideToMove].MG - RookMobilityBonus[rookMobilitySideNotToMove].MG,
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
    /// <param name="fen"></param>
    [TestCase("4k2r/p6p/8/8/8/8/2P4P/1R2K3 w - - 0 1", 10, 2)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k2r1/p4p2/8/8/8/8/P6P/R2K4 b - - 0 1", 10, 2)]
    public void StaticEvaluation_OpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }
        Assert.AreEqual(OpenFileRookBonus.MG
            + RookMobilityBonus[rookMobilitySideToMove].MG - RookMobilityBonus[rookMobilitySideNotToMove].MG,
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
    /// <param name="fen"></param>
    [TestCase("4k2r/p6r/7p/8/8/8/R2P3P/R2K4 w - - 0 1", 7, 6)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("4k2r/p3p2r/8/8/8/P7/R6P/R2K4 b - - 0 1", 7, 6)]
    public void StaticEvaluation_DoubleSemiOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual((2 * SemiOpenFileRookBonus.MG)
            + RookMobilityBonus[rookMobilitySideToMove].MG - RookMobilityBonus[rookMobilitySideNotToMove].MG,
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
    /// <param name="fen"></param>
    [TestCase("1r2k3/1r5p/p7/8/8/P7/R6P/R3K3 w - - 0 1", 6, 12)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    [TestCase("3k3r/p6r/7p/8/8/7P/P5R1/3K2R1 b - - 0 1", 6, 12)]
    public void StaticEvaluation_DoubleOpenFileRookBonus(string fen, int rookMobilitySideToMove, int rookMobilitySideNotToMove)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.R)
            - AdditionalPieceEvaluation(position, Piece.r);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual((-2 * OpenFileRookBonus.MG)
            + RookMobilityBonus[rookMobilitySideToMove].MG
            - RookMobilityBonus[rookMobilitySideNotToMove].MG,
            evaluation);
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
    [Ignore("Broken by virtual king mobility")]
    public void StaticEvaluation_SemiOpenFileKingPenalty(string fen)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalKingEvaluation(position, Piece.K)
            - AdditionalKingEvaluation(position, Piece.k);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(SemiOpenFileKingPenalty.EG, evaluation);
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
        Position position = new Position(fen);
        int evaluation = AdditionalKingEvaluation(position, Piece.K)
            - AdditionalKingEvaluation(position, Piece.k);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(OpenFileKingPenalty.EG, evaluation);
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
    /// <param name="fen"></param>
    [TestCase("6k1/pp1p2pp/8/8/8/8/P1PP2PP/1K6 w - - 0 1")]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
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

        Assert.AreEqual(surroundingPieces * KingShieldBonus.EG, evaluation);
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
    /// <param name="sideToMoveMobilityCount"></param>
    /// <param name="nonSideToMoveMobilityCount"></param>
    [TestCase("n3k3/1p6/8/3b4/3B4/8/6P1/4K2N w - - 0 1", 13, 11)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="sideToMoveMobilityCount"></param>
    /// <param name="nonSideToMoveMobilityCount"></param>
    [TestCase("n2k4/1p6/8/4b3/4B3/8/6P1/3K3N b - - 0 1", 13, 11)]
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
    /// <param name="sideToMoveMobilityCount"></param>
    /// <param name="nonSideToMoveMobilityCount"></param>
    [TestCase("4k3/1p6/2p5/3b4/3B4/5P2/6P1/4K3 w - - 0 1", 13, 9)]
    /// <summary>
    /// Previous one mirrored
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="sideToMoveMobilityCount"></param>
    /// <param name="nonSideToMoveMobilityCount"></param>
    [TestCase("3k4/1p6/2p5/4b3/4B3/5P2/6P1/3K4 b - - 0 1", 13, 9)]
    public void StaticEvaluation_BishopMobility(string fen, int sideToMoveMobilityCount, int nonSideToMoveMobilityCount)
    {
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.B)
            - AdditionalPieceEvaluation(position, Piece.b);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(BishopMobilityBonus[sideToMoveMobilityCount].MG - BishopMobilityBonus[nonSideToMoveMobilityCount].MG, evaluation);
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
        Position position = new Position(fen);
        int evaluation = AdditionalPieceEvaluation(position, Piece.Q)
            - AdditionalPieceEvaluation(position, Piece.q);

        if (position.Side == Side.Black)
        {
            evaluation = -evaluation;
        }

        Assert.AreEqual(mobilityDifference * QueenMobilityBonus.MG, evaluation);
    }

    /// <summary>
    /// https://github.com/lynx-chess/Lynx/pull/510
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="expectedStaticEvaluation"></param>
    [TestCase("QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QPPPPPPP/K6k b - - 0 1", MinEval, IgnoreReason = "Packed eval reduces max eval to a short, so over Short.MaxValue it overflows and produces unexpected results")]
    [TestCase("QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QPPPPPPP/K5k1 w - - 0 1", MaxEval, IgnoreReason = "Packed eval reduces max eval to a short, so over Short.MaxValue it overflows and produces unexpected results")]
    [TestCase("8/QQQQQQ1/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQ6/K6k b - - 0 1", MinEval, IgnoreReason = "It's just a pain to maintain this with bucketed PSQT tuning")]
    [TestCase("8/QQQQQQ1/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQQQQQQQ/QQ6/K5k1 w - - 0 1", MaxEval, IgnoreReason = "It's just a pain to maintain this with bucketed PSQT tuning")]
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
    [TestCase("1k6/8/8/8/8/8/1R6/1K6 w - - 0 1", false, 2, "R")]
    [TestCase("rk6/8/8/8/8/8/8/1K6 w - - 0 1", false, 2, "r")]
    [TestCase("1k6/8/8/8/8/8/1Q6/1K6 w - - 0 1", false, 4, "Q")]
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
        Assert.AreEqual(mg, Position.TaperedEvaluation(new(mg, eg), 24));
        Assert.AreEqual(eg, Position.TaperedEvaluation(new(mg, eg), 0));
    }

    private static int AdditionalPieceEvaluation(Position position, Piece piece)
    {
        var bitBoard = position.PieceBitBoards[(int)piece];
        int eval = 0;

        while (!bitBoard.Empty())
        {
            var pieceSquareIndex = bitBoard.GetLS1BIndex();
            bitBoard.ResetLS1B();
            eval += Utils.UnpackMG(position.AdditionalPieceEvaluation(pieceSquareIndex, (int)piece));
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

        var bitBoard = position.PieceBitBoards[(int)piece].GetLS1BIndex();

        return Utils.UnpackEG(piece == Piece.K
            ? position.KingAdditionalEvaluation(bitBoard, Side.White)
            : position.KingAdditionalEvaluation(bitBoard, Side.Black));
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
