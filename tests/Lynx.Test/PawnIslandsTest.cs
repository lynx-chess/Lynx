using Lynx.ConstantGenerator;
using Lynx.Model;
using NUnit.Framework;

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

        // Original methodtest
        var pawnIslands = PawnIslandsGenerator.IdentifyIslands(whitePawns);
        Assert.AreEqual(expectedPawnIslands, pawnIslands, "Error in the original method");

        // Generator test
        pawnIslands = Position.CountPawnIslands(whitePawns);
        Assert.AreEqual(expectedPawnIslands, pawnIslands, "Error in the generator/prod logic");
    }
}
