using Lynx.Model;
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
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[4]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[5]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[6]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition());
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
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[4]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[5]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[6]);
        Assert.True(game.IsThreefoldRepetition());

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition());
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
        Assert.True(game.IsThreefoldRepetition());

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[4]));
        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[5]));

        game.MakeMove(repeatedMoves[6]);
        Assert.True(game.IsThreefoldRepetition());

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[6]));

        game.MakeMove(repeatedMoves[7]);
        Assert.True(game.IsThreefoldRepetition());

        // Position with castling rights, lost in move Ke1d1
        winningPosition = new Position("1n2k2r/8/8/8/8/8/4PPPP/1N2K2R w Kk - 0 1");

        game = new Game(winningPosition.FEN());
        repeatedMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.b1, (int)BoardSquare.c3, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.c3, (int)BoardSquare.b1, (int)Piece.N),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n),  // Repetition detected, but that's not what we want to test
                MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.d1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.b8, (int)BoardSquare.c6, (int)Piece.n),
                MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.e1, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c6, (int)BoardSquare.b8, (int)Piece.n)   // Not repetition, due to castling rights removal
            };

        // Act
        foreach (var move in repeatedMoves.Take(7))
        {
            Assert.DoesNotThrow(() => game.MakeMove(move));
        }

        game.MakeMove(repeatedMoves[^1]);
        Assert.False(game.IsThreefoldRepetition());                      // Same position, but white not can't castle

#if DEBUG
        Assert.AreEqual(repeatedMoves.Count, game.MoveHistory.Count);
#endif
        Assert.AreEqual(repeatedMoves.Count + 1, game.PositionHashHistory.Count);

        var eval = winningPosition.StaticEvaluation().Score;
        Assert.AreNotEqual(0, eval);

        Assert.DoesNotThrow(() => game.MakeMove(repeatedMoves[5]));

        game.MakeMove(repeatedMoves[6]);
        Assert.True(game.IsThreefoldRepetition());
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
        Assert.AreEqual(101 + 1, game.PositionHashHistory.Count);

        // If the checkmate is in the move when it's claimed, checkmate remains
        Assert.False(game.Is50MovesRepetition());
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
        Assert.AreEqual(100 + 1, game.PositionHashHistory.Count);

        Assert.True(game.Is50MovesRepetition());
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
        Assert.DoesNotThrow(() => game.MakeMove(nonCaptureOrPawnMoveMoves[2]));

#if DEBUG
        Assert.AreEqual(51, game.MoveHistory.Count);
#endif
        Assert.AreEqual(51 + 1, game.PositionHashHistory.Count);

        Assert.False(game.Is50MovesRepetition());
    }
}
