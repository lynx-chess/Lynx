/*
 * Structs seem to be the way to go. Not only they perform slightly better, but also take only 8 bytes (64 bits), vs classes and records (24 bytes)
 *
 *  |           Method | iterations |         Mean |       Error |      StdDev |       Median | Ratio | RatioSD |    Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |----------------- |----------- |-------------:|------------:|------------:|-------------:|------:|--------:|---------:|------:|------:|----------:|
 *  |           Static |          1 |     5.749 us |   0.0881 us |   0.0904 us |     5.730 us |  1.00 |    0.00 |        - |     - |     - |         - |
 *  |              OOP |          1 |     5.608 us |   0.0433 us |   0.0384 us |     5.613 us |  0.97 |    0.01 |        - |     - |     - |         - |
 *  |        OOP_Class |          1 |     7.920 us |   0.1512 us |   0.1553 us |     7.880 us |  1.38 |    0.02 |   0.3662 |     - |     - |     792 B |
 *  |       OOP_Record |          1 |     8.782 us |   0.2998 us |   0.8840 us |     8.337 us |  1.64 |    0.19 |   0.3662 |     - |     - |     792 B |
 *  |       PureUlongs |          1 |     8.335 us |   0.2499 us |   0.7290 us |     8.044 us |  1.49 |    0.16 |        - |     - |     - |         - |
 *  | OOP_ProperRecord |          1 |     7.134 us |   0.1404 us |   0.2013 us |     7.033 us |  1.25 |    0.03 |   0.3738 |     - |     - |     792 B |
 *  |                  |            |              |             |             |              |       |         |          |       |       |           |
 *  |           Static |         10 |    56.293 us |   0.2472 us |   0.2064 us |    56.271 us |  1.00 |    0.00 |        - |     - |     - |         - |
 *  |              OOP |         10 |    56.016 us |   0.3428 us |   0.3206 us |    56.064 us |  0.99 |    0.01 |        - |     - |     - |         - |
 *  |        OOP_Class |         10 |    76.252 us |   0.5704 us |   0.4763 us |    76.317 us |  1.35 |    0.01 |   3.7842 |     - |     - |    7920 B |
 *  |       OOP_Record |         10 |    76.862 us |   0.8994 us |   0.7973 us |    76.568 us |  1.36 |    0.01 |   3.7842 |     - |     - |    7920 B |
 *  |       PureUlongs |         10 |    76.184 us |   1.0866 us |   0.9633 us |    75.919 us |  1.35 |    0.02 |        - |     - |     - |         - |
 *  | OOP_ProperRecord |         10 |    71.070 us |   0.3823 us |   0.3389 us |    71.024 us |  1.26 |    0.01 |   3.7842 |     - |     - |    7920 B |
 *  |                  |            |              |             |             |              |       |         |          |       |       |           |
 *  |           Static |       1000 | 5,841.837 us | 112.9676 us | 146.8898 us | 5,877.205 us |  1.00 |    0.00 |        - |     - |     - |         - |
 *  |              OOP |       1000 | 6,195.253 us | 121.9501 us | 277.7420 us | 6,261.308 us |  1.04 |    0.07 |        - |     - |     - |         - |
 *  |        OOP_Class |       1000 | 8,525.801 us | 286.2689 us | 807.4265 us | 8,264.911 us |  1.43 |    0.09 | 375.0000 |     - |     - |  792000 B |
 *  |       OOP_Record |       1000 | 7,935.784 us | 155.8235 us | 223.4774 us | 7,886.153 us |  1.36 |    0.06 | 375.0000 |     - |     - |  792000 B |
 *  |       PureUlongs |       1000 | 8,296.450 us | 286.6898 us | 840.8114 us | 7,985.670 us |  1.40 |    0.16 |        - |     - |     - |         - |
 *  | OOP_ProperRecord |       1000 | 7,296.272 us | 101.5335 us |  84.7851 us | 7,270.618 us |  1.26 |    0.03 | 375.0000 |     - |     - |  792000 B |
*/

#pragma warning disable RCS1118 // Mark local variable as const.

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class BitBoard_Struct_ReadonlyStruct_Class_Record_Benchmark  : BaseBenchmark
{
    private struct BitBoardOps
    {
        public ulong Board { readonly get; private set; }

        public BitBoardOps(ulong b)
        {
            Board = b;
        }

        public void SetBit(int square)
        {
            Board |= (1UL << square);
        }

        public void PopBit(int square)
        {
            Board &= ~(1UL << square);
        }

        #region unrelated

        public readonly int CountBits() => CountBits(Board);
        public readonly int GetLS1BIndex() => GetLS1BIndex(Board);

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion
    }

    private readonly struct BitBoardOpsReadonly
    {
        public ulong Board { get; }

        public BitBoardOpsReadonly(ulong b)
        {
            Board = b;
        }

        public static ulong SetBit(ulong bb, int square)
        {
            return bb | (1UL << square);
        }

        public static ulong PopBit(ulong bb, int square)
        {
            return bb & ~(1UL << square);
        }

        #region unrelated

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion
    }

#pragma warning disable S3260 // Non-derived "private" classes and records should be "sealed"
    private class BitBoardOpsClass
#pragma warning restore S3260 // Non-derived "private" classes and records should be "sealed"
    {
        public ulong Board { get; private set; }
        public BitBoardOpsClass() { }
        public BitBoardOpsClass(ulong b)
        {
            Board = b;
        }

        public void SetBit(int square)
        {
            Board |= (1UL << square);
        }

        public void PopBit(int square)
        {
            Board &= ~(1UL << square);
        }

        public static ulong SetBit(ulong bb, int square)
        {
            return bb | (1UL << square);
        }

        public static ulong PopBit(ulong bb, int square)
        {
            return bb & ~(1UL << square);
        }

        #region unrelated

        public int CountBits() => CountBits(Board);
        public int GetLS1BIndex() => GetLS1BIndex(Board);

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion

    }

#pragma warning disable S3260 // Non-derived "private" classes and records should be "sealed"
    private record BitBoardOpsRecord
#pragma warning restore S3260 // Non-derived "private" classes and records should be "sealed"
    {
        public ulong Board { get; private set; }

        public BitBoardOpsRecord() { }
        public BitBoardOpsRecord(ulong b)
        {
            Board = b;
        }

        public void SetBit(int square)
        {
            Board |= (1UL << square);
        }

        public void PopBit(int square)
        {
            Board &= ~(1UL << square);
        }

        public static ulong SetBit(ulong bb, int square)
        {
            return bb | (1UL << square);
        }

        public static ulong PopBit(ulong bb, int square)
        {
            return bb & ~(1UL << square);
        }

        #region unrelated

        public int CountBits() => CountBits(Board);
        public int GetLS1BIndex() => GetLS1BIndex(Board);

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion
    }

#pragma warning disable S3260 // Non-derived "private" classes and records should be "sealed"
    private record BitBoardOpsProperRecord(ulong Board)
#pragma warning restore S3260 // Non-derived "private" classes and records should be "sealed"
    {
        public static ulong SetBit(ulong bb, int square)
        {
            return bb | (1UL << square);
        }

        public static ulong PopBit(ulong bb, int square)
        {
            return bb & ~(1UL << square);
        }

        #region unrelated

        public int CountBits() => CountBits(Board);
        public int GetLS1BIndex() => GetLS1BIndex(Board);

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion
    }

    internal static void SizeTest()
    {
        List<ulong> ulongs = new(10_000);
        List<BitBoardOpsClass> classes = new(10_000);
        List<BitBoardOpsRecord> records = new(10_000);
        List<BitBoardOpsProperRecord> properRecords = new(10_000);
        List<BitBoardOps> structs = new(10_000);
        List<BitBoardOpsReadonly> readonlyStructs = new(10_000);

        for (ulong i = 0; i < 10_000; ++i)
        {
            ulongs.Add(i);
            classes.Add(new(i));
            records.Add(new(i));
            properRecords.Add(new(i));
            structs.Add(new(i));
            readonlyStructs.Add(new(i));
        }
    }

    public static IEnumerable<int> Data => [1, 10, 1_000/*, 10_000, 100_000 */];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public void Static(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = new BitBoardOpsReadonly(AttackGenerator.MaskBishopOccupancy(square));

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_Static(index, in occupancyMask);
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void OOP(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = new BitBoardOps(AttackGenerator.MaskBishopOccupancy(square));

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_OOP(index, occupancyMask);
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void OOP_Class(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = new BitBoardOpsClass(AttackGenerator.MaskBishopOccupancy(square));

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_OOP_Class(index, occupancyMask);
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void OOP_Record(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = new BitBoardOpsRecord(AttackGenerator.MaskBishopOccupancy(square));

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_OOP_Record(index, occupancyMask);
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void PureUlongs(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = AttackGenerator.MaskBishopOccupancy(square);

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_PureUlongs(index, occupancyMask);
            }
        }
    }
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void OOP_ProperRecord(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            var square = (int)BoardSquare.e1;

            var occupancyMask = new BitBoardOpsProperRecord(AttackGenerator.MaskBishopOccupancy(square));

            var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

            int occupancyIndexes = (1 << relevantBitsCount);

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                _ = SetBishopOrRookOccupancy_OOP_ProperRecord(index, occupancyMask);
            }
        }
    }

    private static BitBoardOpsReadonly SetBishopOrRookOccupancy_Static(int index, in BitBoardOpsReadonly occupancyMask)
    {
        var occ = occupancyMask.Board;
        var bitsInMask = BitBoardOpsReadonly.CountBits(occ);
        var occupancy = 0UL;

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = BitBoardOpsReadonly.GetLS1BIndex(occ);
            occ = BitBoardOpsReadonly.PopBit(occ, squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy = BitBoardOpsReadonly.SetBit(occupancy, squareIndex);
            }
        }

        return new BitBoardOpsReadonly(occupancy);
    }

    private static BitBoardOps SetBishopOrRookOccupancy_OOP(int index, BitBoardOps occupancyMask)
    {
        var bitsInMask = occupancyMask.CountBits();
        var occupancy = new BitBoardOps();

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = occupancyMask.GetLS1BIndex();
            occupancyMask.PopBit(squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy.SetBit(squareIndex);
            }
        }

        return occupancy;
    }

    private static BitBoardOpsClass SetBishopOrRookOccupancy_OOP_Class(int index, BitBoardOpsClass occupancyMask)
    {
        var bitsInMask = occupancyMask.CountBits();
        var occupancyMaskCopy = occupancyMask.Board;
        var occupancy = new BitBoardOpsClass();

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = BitBoardOpsClass.GetLS1BIndex(occupancyMaskCopy);
            occupancyMaskCopy = BitBoardOpsClass.PopBit(occupancyMaskCopy, squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy.SetBit(squareIndex);
            }
        }

        return occupancy;
    }

    private static BitBoardOpsRecord SetBishopOrRookOccupancy_OOP_Record(int index, BitBoardOpsRecord occupancyMask)
    {
        var bitsInMask = occupancyMask.CountBits();
        var occ = occupancyMask.Board;
        var occupancy = new BitBoardOpsRecord();

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = BitBoardOpsRecord.GetLS1BIndex(occ);
            occ = BitBoardOpsRecord.PopBit(occ, squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy.SetBit(squareIndex);
            }
        }

        return occupancy;
    }

    private static BitBoardOpsProperRecord SetBishopOrRookOccupancy_OOP_ProperRecord(int index, BitBoardOpsProperRecord occupancyMask)
    {
        var bitsInMask = occupancyMask.CountBits();
        var occ = occupancyMask.Board;
        var occupancy = 0UL;

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = BitBoardOpsProperRecord.GetLS1BIndex(occ);
            occ = BitBoardOpsProperRecord.PopBit(occ, squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy = BitBoardOpsProperRecord.SetBit(occupancy, squareIndex);
            }
        }

        return new BitBoardOpsProperRecord(occupancy);
    }

    private static ulong SetBishopOrRookOccupancy_PureUlongs(int index, ulong occupancyMask)
    {
        var bitsInMask = BitBoardOpsClass.CountBits(occupancyMask);
        var occupancy = 0UL;

        // Loop over the range of bits within attack mask
        for (int count = 0; count < bitsInMask; ++count)
        {
            // Extract LS1B and reset it
            int squareIndex = BitBoardOpsClass.GetLS1BIndex(occupancyMask);
            occupancyMask = BitBoardOpsClass.PopBit(occupancyMask, squareIndex);

            // Make sure occupancy is on board
            if ((index & (1 << count)) != default)
            {
                // Update occupancy
                occupancy = BitBoardOpsProperRecord.SetBit(occupancy, squareIndex);
            }
        }

        return occupancy;
    }
}

#pragma warning restore RCS1118 // Mark local variable as const.