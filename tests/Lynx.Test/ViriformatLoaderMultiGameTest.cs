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
            buf[25] = 33;       
            BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(26,2), 66);
            return buf;
        }

        var b1 = BuildSimpleBoard();
        var b2 = BuildSimpleBoard();

        // Each game must be followed by a terminator 4-byte pair of zeros
        var streamBytes = b1.Concat(new byte[4]).Concat(b2).Concat(new byte[4]).ToArray();

        using var ms = new MemoryStream(streamBytes);
        // write to temp file and invoke loader which writes out an .epd file
        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tmp, ms.ToArray());
            ViriformatLoader.LoadFile(tmp);
            var epd = File.ReadAllText(tmp + ".epd");
            // split games by blank line
            var games = epd.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(2, games.Length);
            // each game's first non-empty line contains initial FEN
            string firstFen = games[0].Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string secondFen = games[1].Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.IsTrue(firstFen.StartsWith("4k3/") && secondFen.StartsWith("4k3/"));
            // No move-score lines after initial line for both games
            foreach (var g in games)
            {
                var lines = g.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                // only one line (initial) expected
                Assert.AreEqual(1, lines.Length);
            }
        }
        finally
        {
            try { File.Delete(tmp); } catch { }
            try { File.Delete(tmp + ".epd"); } catch { }
        }
    }
}
