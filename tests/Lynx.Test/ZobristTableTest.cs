using Lynx.Model;
using System;
using NUnit.Framework;

namespace Lynx.Test
{
    public class ZobristTableTest
    {
        private readonly long[,] _zobristTable = ZobristTable.Initialize();

        [Test]
        public void XorBehavior()
        {
            foreach (var hash in _zobristTable)
            {
                var n = Random.Shared.NextInt64();

                var xored = n ^ hash;

                var original = xored ^ hash;

                Assert.AreEqual(n, original);
            }
        }

        [Test]
        public void ReproducibleZobristTable()
        {
            var anotherZobristTable = ZobristTable.Initialize();

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    Assert.AreEqual(_zobristTable[squareIndex, pieceIndex], anotherZobristTable[squareIndex, pieceIndex]);
                }
            }
        }

        [Test]
        public void PieceHash()
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    Assert.AreEqual(_zobristTable[squareIndex, pieceIndex], ZobristTable.PieceHash(squareIndex, pieceIndex));
                }
            }
        }

        [Test]
        public void EnPassantHash()
        {
            foreach (var enPassantSquare in Constants.EnPassantCaptureSquares.Keys)
            {
                var file = enPassantSquare % 8;
                Assert.AreEqual(_zobristTable[file, (int)Piece.P], ZobristTable.EnPassantHash(enPassantSquare));
            }

#if DEBUG
            for (int square = 0; square < 64; ++square)
            {
                var rank = square / 8;
                if (rank != 2 && rank != 5)
                {
                    Assert.Throws<ArgumentException>(() => ZobristTable.EnPassantHash(square));
                }
            }
#endif
        }

        [Test]
        public void SideHash()
        {
            Assert.AreEqual(_zobristTable[(int)BoardSquare.h8, (int)Piece.p], ZobristTable.SideHash());
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
            var positionWithoutCastlingRights = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b - - 0 1");
            var positionWithoutCastlingRightsHash = ZobristTable.PositionHash(positionWithoutCastlingRights);

            var position = new Position(fen);
            var positionHash = ZobristTable.PositionHash(position);

            var castleHash = ZobristTable.CastleHash(position.Castle);

            Assert.AreEqual(positionWithoutCastlingRightsHash, positionHash ^ castleHash);
        }
    }
}
