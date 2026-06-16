using Lynx.Datagen;
using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Datagen;

[Explicit]
[Category(Categories.Datagen)]
public class ViriformatFilterTest
{
    [Test]
    public void MinPieces_FiltersWhenTooFew()
    {
        var position = new Position("8/8/8/8/8/8/4k3/4K3 w - - 0 1");
        var filter = new ViriformatFilter { MinPly = 0, MinPieces = 3 }; // position has 2 kings, require 3 to avoid
        var mv = MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.a2, (int)Piece.P);
        var rng = new Random(1);

        Assert.IsTrue(filter.ShouldDrop(mv, 0, position, 1, 0, rng));
    }

    [Test]
    public void MaxEval_FiltersWhenEvalTooLarge()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, MaxEval = 100 };
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);
        var rng = new Random(1);

        Assert.IsTrue(filter.ShouldDrop(mv, 200, position, 1, 0, rng));
    }

    [Test]
    public void FilterTactical_CaptureAndPromotion_AreFiltered()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, FilterTactical = true };
        var rng = new Random(1);

        // capture (captured piece != None)
        var cap = MoveExtensions.EncodeCapture((int)BoardSquare.e2, (int)BoardSquare.e7, (int)Piece.P, (int)Piece.p);
        Assert.IsTrue(filter.ShouldDrop(cap, 0, position, 1, 0, rng));

        // promotion (promoted piece set)
        var promo = MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.a8, (int)Piece.P, (int)Piece.Q);
        Assert.IsTrue(filter.ShouldDrop(promo, 0, position, 1, 0, rng));
    }

    [Test]
    public void FilterCheck_FiltersWhenInCheck()
    {
        // Use a position known to be in check for side to move (from Position tests)
        var fen = "r1bqk1nr/pppp2pp/2n2p2/4p3/1bB1P3/3P4/PPP2PPP/RNBQK1NR w KQkq - 0 1"; // white to move and in check per tests
        var position = new Position(fen);
        var filter = new ViriformatFilter { MinPly = 0, FilterCheck = true };
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);
        var rng = new Random(1);

        Assert.IsTrue(position.IsInCheck());
        Assert.IsTrue(filter.ShouldDrop(mv, 0, position, 1, 0, rng));
    }

    [Test]
    public void FilterCastling_FiltersCastleMoves()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, FilterCastling = true };
        var rng = new Random(1);

        var castle = MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K);
        Assert.IsTrue(filter.ShouldDrop(castle, 0, position, 1, 0, rng));
    }

    [Test]
    public void RandomFenSkipping_ProbabilityOne_AlwaysFilters()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, RandomFenSkipping = true, RandomFenSkipProbability = 1.0 };
        var rng = new Random(42);
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);

        Assert.IsTrue(filter.ShouldDrop(mv, 0, position, 1, 0, rng));
    }

    [Test]
    public void MaterialCountFiltered_UsesProbabilityArray()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, MaterialCountFiltered = true };
        // set probability for current piece count to 1.0 to guarantee filtering
        int idx = Math.Min(position.CountPieces(), 32);
        filter.MaterialCountProbabilities[idx] = 1.0;
        var rng = new Random(7);
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);

        Assert.IsTrue(filter.ShouldDrop(mv, 0, position, 1, 0, rng));
    }

    [Test]
    public void MaxEvalIncorrectness_FiltersDrawAndNegativeWinnerPov()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, MaxEvalIncorrectness = 50 };
        var rng = new Random(1);
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);

        // draw with large eval
        Assert.IsTrue(filter.ShouldDrop(mv, 100, position, (byte)1, 0, rng));

        // winner POV negative large magnitude also filtered
        Assert.IsTrue(filter.ShouldDrop(mv, -200, position, (byte)2, 0, rng));
    }

    [Test]
    public void WdlFiltered_WdlOutcomeLowChance_IsFiltered()
    {
        var position = new Position(Constants.InitialPositionFEN);
        var filter = new ViriformatFilter { MinPly = 0, WdlFiltered = true };
        var rng = new Random(2);
        var mv = MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P);

        // Choose a large positive eval but a WDL byte indicating 'loss' (0).
        // For a large positive eval the model should predict near-zero chance for 'loss' so the filter will drop it.
        Assert.IsTrue(filter.ShouldDrop(mv, 10_000, position, (byte)0, 0, rng));
    }

    [Test]
    public void InitialPosition_WithNonZeroScore_IsFilteredWhenShouldFilterTrue()
    {
        // Build a simple packed board with non-zero eval at bytes 28..29
        Span<byte> buf = stackalloc byte[32];
        // Put a minimal occupancy with two kings
        ulong occupancy = (1UL << 4) | (1UL << 60);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buf[0..8], occupancy);
        buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);
        // stm_ep
        buf[24] = 64;
        buf[25] = 0;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buf[26..28], 1);
        // eval non-zero
        System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(buf[28..30], 123);

        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tmp, buf.ToArray());

            var filter = new ViriformatFilter { MinPly = 1000 }; // will filter out ply=0
            ViriformatLoader.LoadFile(tmp, filter);

            var lines = File.ReadAllLines(tmp + ".epd").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            Assert.AreEqual(0, lines.Length);
        }
        finally
        {
            try { File.Delete(tmp); } catch { }
            try { File.Delete(tmp + ".epd"); } catch { }
        }
    }

    [Test]
    public void InitialPosition_WithNonZeroScore_IsEmittedWhenUnrestricted()
    {
        Span<byte> buf = stackalloc byte[32];
        ulong occupancy = (1UL << 4) | (1UL << 60);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buf[0..8], occupancy);
        buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);
        buf[24] = 64;
        buf[25] = 0;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buf[26..28], 1);
        System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(buf[28..30], 123);

        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tmp, buf.ToArray());
            var filter = ViriformatFilter.Unrestricted;
            ViriformatLoader.LoadFile(tmp, filter);
            var lines = File.ReadAllLines(tmp + ".epd").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            Assert.AreEqual(1, lines.Length);
        }
        finally
        {
            try { File.Delete(tmp); } catch { }
            try { File.Delete(tmp + ".epd"); } catch { }
        }
    }

    [Test]
    public void InitialPosition_WithZeroScore_IsNotEmittedEvenWhenUnrestricted()
    {
        Span<byte> buf = stackalloc byte[32];
        ulong occupancy = (1UL << 4) | (1UL << 60);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buf[0..8], occupancy);
        buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);
        buf[24] = 64;
        buf[25] = 0;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buf[26..28], 1);
        System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(buf[28..30], 0); // Zero score

        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tmp, buf.ToArray());
            var filter = ViriformatFilter.Unrestricted;
            ViriformatLoader.LoadFile(tmp, filter);
            var lines = File.ReadAllLines(tmp + ".epd").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            Assert.AreEqual(0, lines.Length);
        }
        finally
        {
            try { File.Delete(tmp); } catch { }
            try { File.Delete(tmp + ".epd"); } catch { }
        }
    }
}
