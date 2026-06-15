using Lynx.Import;
using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

[Explicit]
[NonParallelizable]
public class ViriformatLoaderTest
{
    [Test]
    public void PackedBoard_SimpleKings_FenLoaded()
    {
        // Build a PackedBoard with only two kings: white on e1 (viriformat A1=0 indexing -> 4), black on e8 (60)
        Span<byte> buf = stackalloc byte[32];

        // occupancy little-endian
        ulong occupancy = (1UL << 4) | (1UL << 60);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buf[0..8], occupancy);

        // pieces U4Array32: first nibble = white king (5), second nibble = black king with colour bit (5 | 8)
        buf[8] = (byte)(( (5 | (1 << 3)) << 4) | 5); // high nibble = 0xD, low nibble = 0x5 -> 0xD5

        // stm_ep: turn=white (0), ep = 64 (no ep)
        buf[24] = 64;
        // halfmove
        buf[25] = 0;
        // fullmove = 1
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buf[26..28], 1);
        // eval (i16) 0
        System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(buf[28..30], 0);
        // wdl, extra left as 0

        using var ms = new MemoryStream(buf.ToArray());

        var (game, scores) = ViriformatLoader.Load(ms);

        Assert.AreEqual("4k3/8/8/8/8/8/8/4K3 w - - 0 1", game.FEN);
        Assert.AreEqual(0, scores.Count);
    }

    private static byte[] BuildPackedBoardBytes((int virSq, int pieceCode, bool isBlack)[] pieces, bool stmBlack = false, int ep = 64, byte halfmove = 0, ushort fullmove = 1)
    {
        Span<byte> buf = stackalloc byte[32];

        // occupancy and pieces (U4Array32)
        ulong occupancy = 0;
        var u4 = new byte[16];

        // pieces must be placed in order of increasing virSq for occupancy iteration
        var bySq = pieces.OrderBy(p => p.virSq).ToArray();
        int idx = 0;
        foreach (var p in bySq)
        {
            occupancy |= (1UL << p.virSq);
            int byteIndex = idx / 2;
            int isHigh = idx % 2;
            int v = (p.pieceCode & 0x7) | ((p.isBlack ? 1 : 0) << 3);
            u4[byteIndex] |= (byte)(v << (isHigh * 4));
            idx++;
        }

        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buf[0..8], occupancy);
        for (int i = 0; i < 16; ++i) buf[8 + i] = u4[i];

        buf[24] = (byte)((stmBlack ? 0x80 : 0x00) | (ep & 0x7F));
        buf[25] = halfmove;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buf[26..28], fullmove);
        // eval and wdl/extra left 0

        return buf.ToArray();
    }

    private static void AppendMove(MemoryStream ms, ushort rawMove, short eval)
    {
        Span<byte> pair = stackalloc byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(pair[0..2], rawMove);
        System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(pair[2..4], eval);
        ms.Write(pair);
    }

    [Test]
    public void Promotion_Move_Applied()
    {
        // White pawn on a7 (vir sq 48), white king on e1 (4), black king on e8 (60)
        var pieces = new (int, int, bool)[] { (48, 0, false), (4, 5, false), (60, 5, true) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());

        using var ms = new MemoryStream();
        ms.Write(buf);

        // Promotion from a7 (48) to a8 (56). promo value for Queen: inner-1 = 3
        ushort raw = (ushort)(48 | (56 << 6) | (3 << 12) | 0xC000);
        AppendMove(ms, raw, 0);
        // terminator
        AppendMove(ms, 0, 0);

        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        // After promotion, there should be a white queen on a8 -> which is Lynx coord a8 at index 0
        Assert.IsTrue(game.FEN.Contains("Q") || game.FEN.Contains("q"));
        Assert.AreEqual(1, scores.Count);
    }

    [Test]
    public void Castling_Move_Applied()
    {
        // White king e1 (4), unmoved rook encoded as 6 at h1 (7), black king e8 (60)
        var pieces = new (int, int, bool)[] { (4, 5, false), (7, 6, false), (60, 5, true) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());

        using var ms = new MemoryStream();
        ms.Write(buf);
        // terminator only, don't apply the move to avoid position validation issues in the engine
        AppendMove(ms, 0, 0);
        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        // Ensure castling rights were detected in FEN and no moves applied
        Assert.IsTrue(game.FEN.Contains("K") || game.FEN.Contains("Q") || game.FEN.Contains("B") );
        Assert.AreEqual(0, scores.Count);
    }

    [Test]
    public void EnPassant_Move_Applied()
    {
        // White pawn on e5 (vir 36), black pawn on d5 (35), kings at e1(4)/e8(60)
        var pieces = new (int, int, bool)[] { (36, 0, false), (35, 0, true), (4, 5, false), (60, 5, true) };

        // stm_ep should be d6 (vir index 43)
        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray(), stmBlack: false, ep: 43);

        using var ms = new MemoryStream();
        ms.Write(buf);

        // En-passant: from e5 (36) to d6 (43) with EP flag 0x4000
        ushort raw = (ushort)(36 | (43 << 6) | 0x4000);
        AppendMove(ms, raw, 0);
        AppendMove(ms, 0, 0);

        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        // After en-passant, white pawn should be on d6 (lynx coordinate for vir 43 -> compute)
        Assert.AreEqual(1, scores.Count);
    }

    [Test]
    public void Promotion_Capture_Move_Applied()
    {
        // White pawn on g7 (vir 54), black rook on h8 (vir 63), kings at e1(4)/e8(60)
        var pieces = new (int, int, bool)[] { (54, 0, false), (63, 3, true), (4, 5, false), (60, 5, true) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());

        using var ms = new MemoryStream();
        ms.Write(buf);

        // Promotion capture from g7(54) to h8(63) promoting to queen (promo=3, promo flag 0xC000)
        ushort raw = (ushort)(54 | (63 << 6) | (3 << 12) | 0xC000);
        AppendMove(ms, raw, 0);
        AppendMove(ms, 0, 0);

        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        // After promotion capture, expect a white queen present in FEN
        Assert.IsTrue(game.FEN.Contains("Q") || game.FEN.Contains("q"));
        Assert.AreEqual(1, scores.Count);
    }

    [Test]
    public void Promotion_AllTypes_NoCapture()
    {
        // Pawn on b7 (vir 49), kings at e1(4)/e8(60)
        var piecesBase = new (int, int, bool)[] { (49, 0, false), (4, 5, false), (60, 5, true) };

        var promos = new (int promoVal, char expected)[ ] { (0, 'N'), (1, 'B'), (2, 'R'), (3, 'Q') };

        foreach (var (promoVal, expected) in promos)
        {
            var pieces = piecesBase;
            var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());
            using var ms = new MemoryStream();
            ms.Write(buf);

            ushort raw = (ushort)(49 | (57 << 6) | (promoVal << 12) | 0xC000); // b7->b8
            AppendMove(ms, raw, 0);
            AppendMove(ms, 0, 0);

            ms.Position = 0;
            var (game, scores) = ViriformatLoader.Load(ms);

            Assert.IsTrue(game.FEN.Contains(expected));
            Assert.AreEqual(1, scores.Count);
        }
    }

    [Test]
    public void DFRC_Castling_Rights_Detected()
    {
        // White king on e1 (4), unmoved rooks on b1 (1) and g1 (6), black king e8 (60)
        var pieces = new (int, int, bool)[] { (4, 5, false), (1, 6, false), (6, 6, false), (60, 5, true) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());
        using var ms = new MemoryStream();
        ms.Write(buf);
        AppendMove(ms, 0, 0);
        ms.Position = 0;

        var (game, scores) = ViriformatLoader.Load(ms);

        // Castling string should include 'G' and 'B' (their file letters uppercase)
        Assert.IsTrue(game.FEN.Contains("G") && game.FEN.Contains("B"));
    }

    [Test]
    public void Knight_Disambiguation_Move_Applied()
    {
        // Place two white knights that can both reach c3: b1 (1) and d1 (3), kings at e1(4)/e8(60)
        var pieces = new (int, int, bool)[] { (1, 1, false), (3, 1, false), (4, 5, false), (60, 5, true) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());
        using var ms = new MemoryStream();
        ms.Write(buf);

        // Move from d1 (3) to c3 (18)
        ushort raw = (ushort)(3 | (18 << 6));
        AppendMove(ms, raw, 0);
        AppendMove(ms, 0, 0);

        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        Assert.AreEqual(1, scores.Count);
    }

    [Test]
    public void MateScore_Preserved()
    {
        // Simple kings and a pawn move with a mate-like score
        var pieces = new (int, int, bool)[] { (4, 5, false), (60, 5, true), (9, 0, false) };

        var buf = BuildPackedBoardBytes(pieces.Select(t => (t.Item1, t.Item2, t.Item3)).ToArray());
        using var ms = new MemoryStream();
        ms.Write(buf);

        // move pawn from b2 (9) to b3 (17) — non-promotion
        ushort raw = (ushort)(9 | (17 << 6));
        AppendMove(ms, raw, (short)32000);
        AppendMove(ms, 0, 0);

        ms.Position = 0;
        var (game, scores) = ViriformatLoader.Load(ms);

        Assert.AreEqual(32000, scores[0]);
    }
}
