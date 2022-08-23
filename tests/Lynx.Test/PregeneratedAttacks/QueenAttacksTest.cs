using Lynx.Model;
using NUnit.Framework;
using BS = Lynx.Model.BoardSquare;

namespace Lynx.Test.PregeneratedAttacks;

public class QueenAttacksTest
{
    /// <summary>
    /// Implicitly tests <see cref="AttackGenerator.InitializeBishopAttacks"/> and <see cref="Constants.BishopMagicNumbers"/>
    /// </summary>
    /// <param name="bishopSquare"></param>
    /// <param name="occupiedSquares"></param>
    /// <param name="attackedSquares"></param>
    [TestCase(BS.a8, new BS[] { }, new[] {
            BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2, BS.h1,    // Bishop
            BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.h8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2, BS.a1    // Rook
        })]
    [TestCase(BS.a8, new[] { BS.g2, BS.g8, BS.a2 }, new[] {
            BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2,    // Bishop
            BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2    // Rook
        })]
    [TestCase(BS.a8, new[] { BS.b7, BS.b8, BS.a7 }, new[] {
            BS.b7,    // Bishop
            BS.b8, BS.a7 })]    // Rook

    [TestCase(BS.h1, new BS[] { }, new[] {
            BS.g2, BS.f3, BS.e4, BS.d5, BS.c6, BS.b7, BS.a8,    // Bishop
            BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.a1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7, BS.h8 })]    // Rook
    [TestCase(BS.h1, new[] { BS.c6, BS.b1, BS.h7 }, new[] {
            BS.g2, BS.f3, BS.e4, BS.d5, BS.c6,    // Bishop
            BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7})]    // Rook
    [TestCase(BS.h1, new[] { BS.g2, BS.g1, BS.h2 }, new[] {
            BS.g2,    // Bishop
            BS.g1, BS.h2 })]    // Rook

    [TestCase(BS.a1, new BS[] { }, new[] {
            BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7, BS.h8,    // Bishop
            BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.h1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7, BS.a8 })]    // Rook
    [TestCase(BS.a1, new[] { BS.g7, BS.g1, BS.a7 }, new[] {
            BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7,    // Bishop
            BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7})]    // Rook
    [TestCase(BS.a1, new[] { BS.b2, BS.b1, BS.a2 }, new[] {
            BS.b2,    // Bishop
            BS.b1, BS.a2 })]    // Rook

    [TestCase(BS.h8, new BS[] { }, new[] {
            BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2, BS.a1,    // Bishop
            BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.a8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2, BS.h1})]    // Rook
    [TestCase(BS.h8, new[] { BS.b2, BS.b8, BS.h2 }, new[] {
            BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2,    // Bishop
            BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2 })]    // Rook
    [TestCase(BS.h8, new[] { BS.g7, BS.g8, BS.h7 }, new[] {
            BS.g7,    // Bishop
            BS.g8, BS.h7 })]    // Rook

    [TestCase(BS.d4,
        new[] {
                BS.a7, BS.a7, BS.g7, BS.b2, BS.e3,    // Bishop
                BS.d3, BS.d2, BS.b4, BS.d7, BS.h4},    // Rook
        new[] {
                BS.a7, BS.b6, BS.c5, BS.e3, BS.b2, BS.c3, BS.e5, BS.f6, BS.g7,    // Bishop
                BS.b4, BS.c4, BS.e4, BS.f4, BS.g4, BS.h4, BS.d7, BS.d6, BS.d5, BS.d3})]    // Rook
    public void GetQueenAttacks(BS bishopSquare, BS[] occupiedSquares, BS[] attackedSquares)
    {
        // Arrange
        var occupancy = BitBoardExtensions.Initialize(occupiedSquares);

        // Act
        var attacks = Attacks.QueenAttacks((int)bishopSquare, occupancy);

        // Assert
        ValidateAttacks(attackedSquares, attacks);

        // Act
        var bishopAttacks = Attacks.BishopAttacks((int)bishopSquare, occupancy);
        var rookAttacks = Attacks.RookAttacks((int)bishopSquare, occupancy);
        attacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);

        // Assert
        ValidateAttacks(attackedSquares, attacks);

        static void ValidateAttacks(BS[] attackedSquares, BitBoard attacks)
        {
            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.AreEqual(default(BitBoard), attacks);
        }
    }
}
