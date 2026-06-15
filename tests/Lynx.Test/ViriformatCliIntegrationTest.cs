using Lynx.Cli;
using NUnit.Framework;
using System.Buffers.Binary;

namespace Lynx.Test;

public class ViriformatCliIntegrationTest
{
    [Test]
    public async Task Runner_Loads_Viriformat_File_And_Exits()
    {
        // Build a minimal PackedBoard with two kings (white e1, black e8)
        var buf = new byte[32];
        ulong occupancy = (1UL << 4) | (1UL << 60);
        BinaryPrimitives.WriteUInt64LittleEndian(buf.AsSpan(0,8), occupancy);

        // U4Array32: first nibble white king (5), second nibble black king with colour bit
        buf[8] = (byte)(((5 | (1 << 3)) << 4) | 5);

        // stm_ep: turn=white (0), ep = 64 (no ep)
        buf[24] = 64;
        // halfmove
        buf[25] = 33;
        // fullmove = 66
        BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(26,2), 66);

        // Append terminator move pair (4 zero bytes)
        var file = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(file, buf.Concat(new byte[4]).ToArray());

            // Run runner with --load-viriformat <file> and then 'quit' to ensure listener exits
            var args = new string[] { "--load-viriformat", file, "quit" };

            var runTask = Task.Run(() => Runner.Run(args));

            // Wait for runner to complete or timeout
            var completed = await Task.WhenAny(runTask, Task.Delay(5000));
            Assert.IsTrue(completed == runTask, "Runner did not exit within timeout");

            // Ensure the task completed successfully
            await runTask; // will throw if the runner faulted
        }
        finally
        {
            try { File.Delete(file); } catch { }
        }
    }
}
