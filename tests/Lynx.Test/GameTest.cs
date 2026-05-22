using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test;

public class GameTest : BaseTest
{
    [Test]
    public void IsThreefoldRepetition_1()
    {
        const string winningPosition = "7k/8/5KR1/8/8/8/5R2/8 w - - 0 1";

        var game = new Game(winningPosition);
        var repeatedMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),  // Repetition detected for the first time
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k),
            };

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[0]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[1]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[2]));

        game.MakeMove(repeatedMoves[3]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[4]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[5]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[6]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));
    }

    [Test]
    public void IsThreefoldRepetition_2()
    {
        // https://lichess.org/MgWVifcK
        const string winningPosition = "6k1/6b1/1p6/2p5/P7/1K4R1/8/r7 b - - 7 52";

        var game = new Game(winningPosition);
        var repeatedMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),      // Repetition detected for the first time
                MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.b1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.a2, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.a1, (int)Piece.r),
                MoveExtensions.Encode((int)BoardSquare.a2, (int)BoardSquare.b3, (int)Piece.K),
            };

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[0]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[1]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[2]));

        game.MakeMove(repeatedMoves[3]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[4]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[5]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[6]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));
    }

    [Test]
    public void IsThreefoldRepetition_CastleRightsRemoval()
    {
        // Arrange

        // Position without castling rights
        var winningPosition = new Position("1n2k2r/8/8/8/8/8/4PPPP/1N2K2R w - - 0 1");

        var game = new Game(winningPosition.FEN());
        var repeatedMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),  // Repetition detected
                MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)
            };

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[0]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[1]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[2]));

        game.MakeMove(repeatedMoves[3]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[4]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[5]));

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[6]));
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));

        // Position with castling rights, lost in move Ke1d1
        winningPosition = new Position("1n2k2r/8/8/8/8/8/4PPPP/1N2K2R w Kk - 0 1");

        game = new Game(winningPosition.FEN());
        repeatedMoves =
        [
            MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
            MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
            MoveExtensions.Encode((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
            MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),  // Repetition detected, but that's not what we want to test
            MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
            MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
            MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
            MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)   // Not repetition, due to castling rights removal
        ];

        // Act
        foreach (var move in repeatedMoves.Take(7))
        {
            Assert.DoesNotThrow(() => game.MakeMove(move));
        }

        game.MakeMove(repeatedMoves[^1]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.False(game.IsThreefoldRepetition(1));

#if DEBUG
        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);
#endif
        Assert.AreEqual(repeatedMoves.Count + 1, game.PositionHashHistoryLength());

        var eval = winningPosition.StaticEvaluation().Score;
        Assert.AreNotEqual(0, eval);

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[4]));

        game.MakeMove(repeatedMoves[5]);
        Assert.False(game.IsThreefoldRepetition(0));
        Assert.True(game.IsThreefoldRepetition(1));
    }

    [Test]
    public void IsThreefoldRepetition_NoTwoFoldAtRoot()
    {
        var game = new Game(Constants.InitialPositionFEN);
        game.ParsePositionCommand("position fen r7/PK6/8/5k2/R7/8/8/8 b - - 6 101 moves a8g8 a4a5 f5e4 a5a4");

        Assert.False(game.IsThreefoldRepetition(0));

        var engine = GetEngine();
        engine.SetGame(game);

        var result = engine.BestMove(new("go depth 20"));

        Assert.Less(result.Score, 0);
    }

    [TestCase("position startpos moves d2d4 g8f6 c2c4 e7e6 g1f3 d7d5 b1c3 f8e7 c1f4 e8g8" +
        " e2e3 c7c5 d4c5 e7c5 a2a3 a7a6 d1d2 d5c4 f1c4 b7b5 c4d3 c8b7 b2b4 b7f3 b4c5 f3g2" +
        " h1g1 g2f3 g1g3 f3c6 f4h6 g7g6 h6f8 d8f8 a3a4 b5b4 c3a2 b4b3 a2c1 f8c5 c1b3 c5d5" +
        " b3a5 d5h1 d3f1 c6d5 d2c3 b8d7 c3c7 h1e4 f1c4 e4c2 a1a2 c2c1 e1e2 d7e5 c7e5 d5c4" +
        " a5c4 c1c4 e2f3 c4a2 e5f6 a2a4 f3g2 a8c8 g3f3 a4d7 f6e5 d7b5 e5d6 a6a5 d6e7 c8f8" +
        " e7d6 a5a4 e3e4 b5c4 f3e3 f8c8 h2h3 c4b5 e3f3 b5g5 f3g3 g5a5 g3d3 a5g5 d3g3 g5b5" +
        " g3f3 b5a5 f3d3 a5a8 d6b4 c8b8 b4c4 a4a3 d3d2 b8b2 d2b2 a3b2 c4b4 a8a2 b4b8 g8g7" +
        " b8e5 g7g8 e5b8 g8g7 b8e5 f7f6 e5c7 g7g8 c7b8 g8f7 b8c7 f7g8 c7b8 g8g7")]
    public void NoThreefoldRepetition(string positionCommand)
    {
        for (int depth = 1; depth < 5; ++depth)
        {
            var engine = GetEngine();
            engine.AdjustPosition(positionCommand);

            var bestMove = engine.BestMove(new GoCommand($"go depth {depth}"));
            Assert.NotZero(bestMove.Score);
        }
    }

    [Test]
    public void FiftyMovesRule_1()
    {
        const string winningPosition = "7k/8/5KR1/8/8/8/5R2/8 w - - 0 1";

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
            Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

        Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[2]));
        Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[3]));
        Assert.DoesNotThrow(() => game.MakeMove(MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.h2, (int)Piece.R)));   // Mate on move 51

#if DEBUG
        Assert.AreEqual(101, game.MoveHistory.Count);
#endif
        Assert.AreEqual(101 + 1, game.PositionHashHistoryLength());

        // If the checkmate is in the move when it's claimed, checkmate remains

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        Assert.False(game.Is50MovesRepetition(ref evaluationContext));
    }

    [Test]
    public void FiftyMovesRule_2()
    {
        // https://lichess.org/MgWVifcK
        const string winningPosition = "6k1/6b1/1p6/2p5/P7/1K4R1/8/r7 b - - 7 52";

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
            Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

#if DEBUG
        Assert.AreEqual(100, game.MoveHistory.Count);
#endif
        Assert.AreEqual(100 + 1, game.PositionHashHistoryLength());

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        Assert.True(game.Is50MovesRepetition(ref evaluationContext));
    }

    [Test]
    public void FiftyMovesRule_Promotion()
    {
        // https://lichess.org/MgWVifcK
        const string winningPosition = "6k1/6b1/1p6/2p5/P7/1K4R1/7p/r7 b - - 7 52";

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
            Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count]));
        }

        Assert.DoesNotThrow(() => game.MakeMove(MoveExtensions.EncodePromotion((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)Piece.q)));   // Promotion
        Assert.DoesNotThrow(() => game.MakeMove(MoveExtensions.Encode((int)BoardSquare.b3, (int)BoardSquare.c4, (int)Piece.K)));
        Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[0]));

#if DEBUG
        Assert.AreEqual(51, game.MoveHistory.Count);
#endif
        Assert.AreEqual(51 + 1, game.PositionHashHistoryLength());

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        Assert.False(game.Is50MovesRepetition(ref evaluationContext));
    }
}
