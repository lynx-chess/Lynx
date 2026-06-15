using NUnit.Framework;
using System.Buffers.Binary;
using System.IO;

namespace Lynx.Test;

public class ViriformatLoaderParseTest
{
    private static byte[] BuildPackedBoard(int eval = 0, byte wdl = 0)
    {
        var buf = new byte[32];
        // minimal occupancy: kings on e1 (vir 4) and e8 (60)
        ulong occupancy = (1UL << 4) | (1UL << 60);
        BinaryPrimitives.WriteUInt64LittleEndian(buf.AsSpan(0, 8), occupancy);
        // pieces: place black king high nibble, white king low nibble
        buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);
        // stm_ep = no ep, white to move
        buf[24] = 64;
        // halfmove
        buf[25] = 0;
        // fullmove = 1
        BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(26, 2), 1);
        // eval
        BinaryPrimitives.WriteInt16LittleEndian(buf.AsSpan(28, 2), (short)eval);
        // wdl
        buf[30] = wdl;
        // extra left as 0
        return buf;
    }

    [Test]
    public void ParseEvalAndWDL_ReturnsScoreAndWdlByte()
    {
        var buf = BuildPackedBoard(eval: 1234, wdl: 2);
        var (score, wdl) = Lynx.ViriformatLoader.ParseEvalAndWDL(buf);
        Assert.AreEqual(1234, score);
        Assert.AreEqual(2, wdl);
    }

    [Test]
    public void MapWdlToResult_MatchesSpec()
    {
        Assert.AreEqual("0.0", Lynx.ViriformatLoader.MapWdlToResult(0));
        Assert.AreEqual("0.5", Lynx.ViriformatLoader.MapWdlToResult(1));
        Assert.AreEqual("1.0", Lynx.ViriformatLoader.MapWdlToResult(2));
        Assert.AreEqual("*", Lynx.ViriformatLoader.MapWdlToResult(99));
    }
}
