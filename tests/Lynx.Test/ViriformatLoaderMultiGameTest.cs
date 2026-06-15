using Lynx.Model;
using NUnit.Framework;
using System.Buffers.Binary;

namespace Lynx.Test;

public class ViriformatLoaderMultiGameTest
{
    [Test]
    public void LoadAll_MultipleGamesInStream()
    {
        // Build two minimal PackedBoard games: both only kings in default places
        byte[] BuildSimpleBoard()
        {
            var buf = new byte[32];
            ulong occupancy = (1UL << 4) | (1UL << 60);
            BinaryPrimitives.WriteUInt64LittleEndian(buf.AsSpan(0,8), occupancy);
            buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);
            buf[24] = 64;
            buf[25] = 0;
            BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(26,2), 1);
            return buf;
        }

        var b1 = BuildSimpleBoard();
        var b2 = BuildSimpleBoard();

        // Each game must be followed by a terminator 4-byte pair of zeros
        var streamBytes = b1.Concat(new byte[4]).Concat(b2).Concat(new byte[4]).ToArray();

        using var ms = new MemoryStream(streamBytes);
        var all = ViriformatLoader.LoadAll(ms);

        Assert.AreEqual(2, all.Count);
        Assert.IsTrue(all[0].game.FEN == all[1].game.FEN);
        Assert.AreEqual(0, all[0].moveScores.Count);
        Assert.AreEqual(0, all[1].moveScores.Count);
    }
}
