using NUnit.Framework;

namespace Lynx.Test.PregeneratedAttacks;
public class AttackGeneratorTest
{
    [Test]
    public void InitializeKingAttacks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeKingAttacks(), Attacks.KingAttacks);
    }

    [Test]
    public void InitializeKnightAttacks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeKnightAttacks(), Attacks.KnightAttacks);
    }

    [Test]
    public void InitializePawnAttacks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializePawnAttacks(), Attacks.PawnAttacks);
    }

    [Test]
    public void InitializeBishopAttacks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeBishopAttacksAndOccupancy().BishopAttacks, Attacks._bishopAttacks);
    }

    [Test]
    public void InitializeRookAttacks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeRookAttacksAndOccupancy().RookAttacks, Attacks._rookAttacks);
    }

    [Test]
    public void InitializeBishopOccupancyMasks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeBishopAttacksAndOccupancy().BishopOccupancyMasks, Attacks._bishopOccupancyMasks);
    }

    [Test]
    public void InitializeRookOccupancyMasks()
    {
        CollectionAssert.AreEqual(AttackGenerator.InitializeRookAttacksAndOccupancy().RookOccupancyMasks, Attacks._rookOccupancyMasks);
    }
}
