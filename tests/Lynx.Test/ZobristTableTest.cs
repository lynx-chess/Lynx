using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class ZobristTableTest
{
    [Test]
    public void XorBehavior()
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        for (int i = 0; i < 64; ++i)
        {
            for (int j = 0; j < 1; ++j)
            {
                var hash = zobristTable[i][j];

                var n = random.NextUInt64();

                var xored = n ^ hash;

                var original = xored ^ hash;

                Assert.AreEqual(n, original);
            }
        }
    }

    [Test]
    public void ReproducibleZobristTable()
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        var anotherRandom = new LynxRandom(int.MaxValue);
        var anotherZobristTable = ZobristTable.InitializeZobristTable(anotherRandom);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                Assert.AreEqual(zobristTable[squareIndex][pieceIndex], anotherZobristTable[squareIndex][pieceIndex]);
            }
        }
    }

    [Test]
    public void PieceHash()
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                Assert.AreEqual(zobristTable[squareIndex][pieceIndex], ZobristTable.PieceHash(squareIndex, pieceIndex));
            }
        }
    }

    [Test]
    public void EnPassantHash()
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        var enPassantSquares = Constants.EnPassantCaptureSquares.ToArray().Select((item, index) => (item, index)).Where(pair => pair.item != 0).Select(pair => pair.index);

        foreach (var enPassantSquare in enPassantSquares)
        {
            var file = enPassantSquare % 8;
            Assert.AreEqual(zobristTable[file][(int)Piece.P], ZobristTable.EnPassantHash(enPassantSquare));
        }

        Assert.AreEqual(16, enPassantSquares.Count());
    }

    [Test]
    public void SideHash()
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        Assert.AreEqual(zobristTable[(int)BoardSquare.h8][(int)Piece.p], ZobristTable.SideHash());
    }

    [TestCase(Constants.TrickyTestPositionReversedFEN)]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQ - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b Kkq - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b Kq - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b K - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b Qk - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b Qq - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b Q - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b kq - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b k - 0 1")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b q - 0 1")]
    public void CastleHash(string fen)
    {
        var random = new LynxRandom(int.MaxValue);
        var zobristTable = ZobristTable.InitializeZobristTable(random);

        var positionWithoutCastlingRights = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b - - 0 1");
        var positionWithoutCastlingRightsHash = ZobristTable.PositionHash(positionWithoutCastlingRights);

        var position = new Position(fen);
        var positionHash = ZobristTable.PositionHash(position);

        var castleHash = ZobristTable.CastleHash(position.Castle);

        Assert.AreEqual(CalculateCastleHash(position.Castle, zobristTable), castleHash);

        Assert.AreEqual(positionWithoutCastlingRightsHash, positionHash ^ castleHash);
    }

    [TestCase(Constants.InitialPositionFEN)]
    [TestCase(Constants.TrickyTestPositionFEN)]
    [TestCase(Constants.TrickyTestPositionReversedFEN)]
    [TestCase(Constants.CmkTestPositionFEN)]
    [TestCase(Constants.ComplexPositionFEN)]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase(Constants.TTPositionFEN)]
    public void PositionHash(string fen)
    {
        var position = new Position(fen);

        var originalHash = OriginalPositionHash(position);
        var currentHash = ZobristTable.PositionHash(position);

        Assert.AreEqual(originalHash, currentHash);
    }

    private static ulong CalculateCastleHash(byte castle, ulong[][] zobristTable)
    {
        ulong combinedHash = 0;

        if ((castle & (int)CastlingRights.WK) != default)
        {
            combinedHash ^= zobristTable[(int)BoardSquare.a8][(int)Piece.p];        // a8
        }

        if ((castle & (int)CastlingRights.WQ) != default)
        {
            combinedHash ^= zobristTable[(int)BoardSquare.b8][(int)Piece.p];        // b8
        }

        if ((castle & (int)CastlingRights.BK) != default)
        {
            combinedHash ^= zobristTable[(int)BoardSquare.c8][(int)Piece.p];        // c8
        }

        if ((castle & (int)CastlingRights.BQ) != default)
        {
            combinedHash ^= zobristTable[(int)BoardSquare.d8][(int)Piece.p];        // d8
        }

        return combinedHash;
    }

    private static ulong OriginalPositionHash(Position position)
    {
        ulong positionHash = 0;

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                if (position.PieceBitboards[pieceIndex].GetBit(squareIndex))
                {
                    positionHash ^= ZobristTable.PieceHash(squareIndex, pieceIndex);
                }
            }
        }

        positionHash ^= ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.SideHash() * (ulong)position.Side
            ^ ZobristTable.CastleHash(position.Castle);

        return positionHash;
    }
}
