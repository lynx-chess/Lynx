using Lynx.ConstantsGenerator;
using Lynx.Model;
using NUnit.Framework;
using System.Numerics;

namespace Lynx.Test;

public class PawnIslandsTest
{
    [TestCase("5k1K/8/8/8/8/8/8/8 w - - 0 1", 0)]

    [TestCase("5k1K/8/8/8/8/8/PPPPPPPP/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/1PPPPPPP/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/PPPPPPP1/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/1PPPPPP1/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/2PPPPPP/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/PPPPPP2/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/2PPP2/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/3PPPPP/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/PPPPP3/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/3PP3/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/P7/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/1P6/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/6P1/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/2P5/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/5P2/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/3P4/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/4P3/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/PP6/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/6PP/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/3PP3/8 w - - 0 1", 1)]
    [TestCase("5k1K/8/8/8/8/8/4P3/8 w - - 0 1", 1)]

    [TestCase("5k1K/8/8/8/8/8/P1PPPPPP/8 w - - 0 1", 2)]
    [TestCase("5k1K/8/8/8/8/8/P1P1PPPP/8 w - - 0 1", 3)]
    [TestCase("5k1K/8/8/8/8/8/P1P1P1PP/8 w - - 0 1", 4)]
    [TestCase("5k1K/8/8/8/8/8/P1P1P2P/8 w - - 0 1", 4)]
    [TestCase("5k1K/8/8/8/8/8/P1P1P1P1/8 w - - 0 1", 4)]

    [TestCase("5k1K/8/8/8/8/8/P1P5/8 w - - 0 1", 2)]
    [TestCase("5k1K/2P5/2P5/2P5/2P5/2P5/P1P5/8 w - - 0 1", 2)]
    [TestCase("5k1K/8/8/8/8/8/P6P/8 w - - 0 1", 2)]
    public void PawnIslandsCount(string fen, int expectedPawnIslands)
    {
        var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
        BitBoard whitePawns = pieces[(int)Piece.P];

        // Original method test
        var pawnIslands = PawnIslandsGenerator.IdentifyIslands(whitePawns);
        Assert.AreEqual(expectedPawnIslands, pawnIslands, "Error in the original method");

        // Generator test
        pawnIslands = CountPawnIslands(whitePawns);
        Assert.AreEqual(expectedPawnIslands, pawnIslands, "Error in the generator");

        // Generator test
        pawnIslands = CountPawnIslandsImproved(whitePawns);
        Assert.AreEqual(expectedPawnIslands, pawnIslands, "Error in the improved method");

        var pawnIslandsBonus = Position.PawnIslands(whitePawns, pieces[(int)Piece.p]);
        Assert.AreEqual(EvaluationParams.PawnIslandsBonus[expectedPawnIslands] - EvaluationParams.PawnIslandsBonus[0], pawnIslandsBonus, "Error in the Position implementation");
    }

    private static int CountPawnIslands(BitBoard pawns)
    {
        int pawnFileBitBoard = 0;

        while (pawns != 0)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

            // BitBoard.SetBit equivalent but for byte instead of ulong
            pawnFileBitBoard |= (1 << Constants.File[squareIndex]);
        }

        return PawnIslandsGenerator.PawnIslandsCount[pawnFileBitBoard];
    }

    private static int CountPawnIslandsImproved(BitBoard pawns)
    {
        byte pawnFileBitBoard = 0;

        while (pawns != 0)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

            // BitBoard.SetBit equivalent but for byte instead of ulong
            pawnFileBitBoard |= (byte)(1 << (squareIndex % 8));
        }

        int shifted = pawnFileBitBoard << 1;
        int starts = pawnFileBitBoard & (~shifted);   // treat shifted’s MSB as 0 implicitly

        return BitOperations.PopCount((uint)starts);
    }
}
