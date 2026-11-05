using Lynx.Model;
using NUnit.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Lynx.EvaluationConstants;

namespace Lynx.Test.Model;

public class TranspositionTableTests
{
    [TestCase(10_000, 1, 10_000)]
    [TestCase(10_000, 5, 10_000)]
    [TestCase(10_000, 3, 10_000)]
    [TestCase(PositiveCheckmateDetectionLimit - 1, 5, PositiveCheckmateDetectionLimit - 1)]
    [TestCase(-10_000, 1, -10_000)]
    [TestCase(-10_000, 3, -10_000)]
    [TestCase(-10_000, 5, -10_000)]
    [TestCase(NegativeCheckmateDetectionLimit + 1, 5, NegativeCheckmateDetectionLimit + 1)]

    [TestCase(CheckMateBaseEvaluation - 5, 2, CheckMateBaseEvaluation - 7)]
    [TestCase(CheckMateBaseEvaluation - 2, 4, CheckMateBaseEvaluation - 6)]

    [TestCase(-CheckMateBaseEvaluation + 5, 2, -CheckMateBaseEvaluation + 7)]
    [TestCase(-CheckMateBaseEvaluation + 2, 4, -CheckMateBaseEvaluation + 6)]
    public void RecalculateMateScores(int evaluation, int depth, int expectedEvaluation)
    {
        Assert.AreEqual(expectedEvaluation, TranspositionTable.RecalculateMateScores(evaluation, depth));
    }

    [TestCase(+19, NodeType.Alpha, +19)]
    [TestCase(+31, NodeType.Beta, +31)]
    public void RecordHash_ProbeHash(int recordedEval, NodeType recordNodeType, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        var staticEval = position.StaticEvaluation().Score;

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval, depth: 5, ply: 3, score: recordedEval, nodeType: recordNodeType, false, move: recordNodeType != NodeType.Alpha ? 1234 : null);

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: 3, out var ttEntry));
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(staticEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - 8)]
    [TestCase(-CheckMateBaseEvaluation + 3)]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, recordedEval, depth: 10, ply: sharedDepth, score: recordedEval, nodeType: NodeType.Exact, false, move: 1234);

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: sharedDepth, out var ttEntry));
        Assert.AreEqual(recordedEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - 8, 5, 4, CheckMateBaseEvaluation - 7)]
    [TestCase(CheckMateBaseEvaluation - 8, 5, 6, CheckMateBaseEvaluation - 9)]
    [TestCase(-CheckMateBaseEvaluation + 8, 5, 4, -CheckMateBaseEvaluation + 7)]
    [TestCase(-CheckMateBaseEvaluation + 8, 5, 6, -CheckMateBaseEvaluation + 9)]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, recordedEval, depth: 10, ply: recordedDeph, score: recordedEval, nodeType: NodeType.Exact, false, move: 1234);

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: probeDepth, out var ttEntry));
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(NodeType.Unknown, 0, 0)]
    [TestCase(NodeType.Exact, 1, 31)]
    [TestCase(NodeType.Alpha, 0, 17)]
    [TestCase(NodeType.Beta, 1, 8)]
    [TestCase(NodeType.Beta, 0, 31)]
    public void AgeTypeWasPvFlags(NodeType nodeType, int wasPv, int age)
    {
        var element = new TranspositionTableElement();

        element.Update(key: 0xABCD, score: -123, staticEval: 456, depth: 17, nodeType, wasPv, move: 789, age);

        Assert.AreEqual(0xABCD, element.Key);
        Assert.AreEqual(-123, element.Score);
        Assert.AreEqual(456, element.StaticEval);
        Assert.AreEqual(17, element.Depth);
        Assert.AreEqual(789, element.Move);
        Assert.AreEqual(nodeType, element.Type);
        Assert.AreEqual(wasPv == 1, element.WasPv);
        Assert.AreEqual(age, element.Age);
    }

    /// <summary>
    /// Every <see cref="Engine"/> keeps its own copy of the <see cref="TranspositionTable"/> struct, and <see cref="Searcher"/> is the only one bumping the age.
    /// </summary>
    [Test]
    public unsafe void BumpAge_IsVisibleThroughCopiesOfTheStruct()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();

            var engineCopy = tt;
            Assert.AreEqual(0, engineCopy.Age);

            tt.BumpAge();
            tt.BumpAge();
            tt.BumpAge();

            Assert.AreEqual(3, tt.Age);
            Assert.AreEqual(3, engineCopy.Age);

            var position = new Position(Constants.InitialPositionFEN);
            engineCopy.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 5, ply: 0, score: 10, NodeType.Exact, wasPv: false, move: 1234);
            Assert.AreEqual(3, FindOnlyRecordedEntry(tt)->Age);

            engineCopy.SaveStaticEval(new Position(Constants.InitialPositionFEN), halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 2, wasPv: false);
            Assert.AreEqual(3, FindOnlyRecordedEntry(tt)->Age);
        }
        finally
        {
            tt.Release();
        }
    }

    /// <summary>
    /// Gives access to the actual (private) Engine._tt field, not to a copy of it
    /// </summary>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_tt")]
    private static extern ref TranspositionTable GetEngineTT(Engine engine);

    /// <summary>
    /// <see cref="TranspositionTable"/> is a struct, so every <see cref="Engine"/> stores its own copy of it.
    /// The copies are independent storage locations, but all of them point to the same native memory.
    /// </summary>
    [Test]
    public unsafe void Engines_HoldIndependentCopiesOfTheStruct_PointingToTheSameMemory()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();

            using var engineA = new Engine(1, SilentChannelWriter<object>.Instance, in tt);
            using var engineB = new Engine(2, SilentChannelWriter<object>.Instance, in tt);

            ref var copyA = ref GetEngineTT(engineA);
            ref var copyB = ref GetEngineTT(engineB);

            // Three different storage locations: the caller's struct and one field per engine
            Assert.IsFalse(Unsafe.AreSame(ref copyA, ref copyB), "Both engines share the same struct instance");
            Assert.IsFalse(Unsafe.AreSame(ref copyA, ref tt), "Engine A holds the caller's struct instance, not a copy");
            Assert.IsFalse(Unsafe.AreSame(ref copyB, ref tt), "Engine B holds the caller's struct instance, not a copy");

            // ... that point to the same native memory
            Assert.AreEqual((ulong)tt.Get(0), (ulong)copyA.Get(0));
            Assert.AreEqual((ulong)tt.Get(0), (ulong)copyB.Get(0));

            // ... which is where the (shared) age lives
            tt.BumpAge();
            Assert.AreEqual(1, copyA.Age);
            Assert.AreEqual(1, copyB.Age);

            // Overwriting one copy doesn't affect the others, so they really are independent instances
            var originalCopyA = copyA;
            try
            {
                copyA = default;

                Assert.AreEqual((nuint)0, copyA.Length);
                Assert.AreEqual(tt.Length, copyB.Length);
                Assert.AreNotEqual((nuint)0, tt.Length);
                Assert.AreEqual((ulong)tt.Get(0), (ulong)copyB.Get(0));
            }
            finally
            {
                copyA = originalCopyA;
            }
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public void BumpAge_CircularBuffer()
    {
        var tt = new TranspositionTable();
        try
        {
            for (int i = 1; i < TranspositionTableElement.MaxAge; ++i)
            {
                tt.BumpAge();
                Assert.AreEqual(i, tt.Age);
            }

            tt.BumpAge();
            Assert.AreEqual(0, tt.Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public void Clear_ResetsAge()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.BumpAge();
            tt.BumpAge();
            Assert.AreEqual(2, tt.Age);

            tt.Clear();

            Assert.AreEqual(0, tt.Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public unsafe void Allocation_IsCacheLineAligned()
    {
        var tt = new TranspositionTable();
        try
        {
            Assert.AreEqual(0UL, (ulong)tt.Get(0) % 64);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public void MaxAge_PowerOfTwo()
    {
        Assert.Zero(TranspositionTableElement.MaxAge & (TranspositionTableElement.MaxAge - 1));
    }

    [Test]
    public unsafe void Bucket_NeverStraddlesCacheLines()
    {
        Assert.AreEqual(Constants.TranspositionTableBucketSize, sizeof(TranspositionTableBucket));
        Assert.LessOrEqual(Constants.TranspositionTableEntriesPerBucket * sizeof(TranspositionTableElement), Constants.TranspositionTableBucketSize);
        Assert.AreEqual(0, 64 % Constants.TranspositionTableBucketSize);

        var tt = new TranspositionTable();
        try
        {
            for (int bucketIndex = 0; bucketIndex < 1_000; ++bucketIndex)
            {
                var first = (ulong)tt.Get(bucketIndex);
                var last = (ulong)tt.Get(bucketIndex, Constants.TranspositionTableEntriesPerBucket - 1) + (ulong)sizeof(TranspositionTableElement) - 1;

                Assert.AreEqual(first / 64, last / 64, $"Bucket {bucketIndex} straddles two cache lines");
                Assert.AreEqual((ulong)(bucketIndex * Constants.TranspositionTableBucketSize), first - (ulong)tt.Get(0));
            }
        }
        finally
        {
            tt.Release();
        }
    }

    /// <summary>
    /// Entries are accessed by indexing a <see cref="TranspositionTableElement"/>* that starts at the beginning of the bucket,
    /// so <see cref="TranspositionTableBucket"/> must declare exactly <see cref="Constants.TranspositionTableEntriesPerBucket"/> entries, one right after the other,
    /// followed by the padding that fills the bucket up to <see cref="Constants.TranspositionTableBucketSize"/>.
    /// If this test fails after changing <see cref="Constants.TranspositionTableEntriesPerBucket"/> or <see cref="Constants.TranspositionTableBucketSize"/>,
    /// add or remove entry fields, and resize the padding, in <see cref="TranspositionTableBucket"/>.
    /// </summary>
    [Test]
    public void Bucket_DeclaresOneFieldPerEntryPlusPadding()
    {
        var fields = typeof(TranspositionTableBucket).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var entryFields = fields.Where(field => field.FieldType == typeof(TranspositionTableElement)).ToArray();
        var paddingFields = fields.Except(entryFields).ToArray();

        var entrySize = Marshal.SizeOf<TranspositionTableElement>();
        var entriesEnd = Constants.TranspositionTableEntriesPerBucket * entrySize;

        Assert.AreEqual(
            Constants.TranspositionTableEntriesPerBucket,
            entryFields.Length,
            $"{nameof(TranspositionTableBucket)} declares {entryFields.Length} entry field(s), but {nameof(Constants.TranspositionTableEntriesPerBucket)} is {Constants.TranspositionTableEntriesPerBucket}");

        var entryOffsets = entryFields
            .Select(field => (int)Marshal.OffsetOf<TranspositionTableBucket>(field.Name))
            .Order()
            .ToArray();
        var expectedEntryOffsets = Enumerable.Range(0, Constants.TranspositionTableEntriesPerBucket)
            .Select(i => i * entrySize)
            .ToArray();

        CollectionAssert.AreEqual(expectedEntryOffsets, entryOffsets, "Entries aren't laid out contiguously from the start of the bucket");

        // Padding goes after the entries and fills the bucket, no more and no less
        var expectedPaddingSize = Constants.TranspositionTableBucketSize - entriesEnd;
        Assert.GreaterOrEqual(expectedPaddingSize, 0, $"{Constants.TranspositionTableEntriesPerBucket} entries don't fit in {Constants.TranspositionTableBucketSize} bytes");

        foreach (var paddingField in paddingFields)
        {
            Assert.GreaterOrEqual((int)Marshal.OffsetOf<TranspositionTableBucket>(paddingField.Name), entriesEnd, $"Padding field {paddingField.Name} overlaps the entries");
        }

        Assert.AreEqual(
            expectedPaddingSize,
            paddingFields.Sum(field => Marshal.SizeOf(field.FieldType)),
            $"Padding should be {expectedPaddingSize} byte(s) ({Constants.TranspositionTableBucketSize} - {Constants.TranspositionTableEntriesPerBucket} * {entrySize})");

        Assert.AreEqual(Constants.TranspositionTableBucketSize, Marshal.SizeOf<TranspositionTableBucket>());
    }

    private static TranspositionTableElement CreateEntry(ushort key, int depth, int age, NodeType nodeType = NodeType.Beta)
    {
        var entry = new TranspositionTableElement();
        entry.Update(key, score: 0, staticEval: 0, depth, nodeType, wasPv: 0, move: null, age);

        return entry;
    }

    /// <summary>
    /// Index of the entry <see cref="TranspositionTable.SelectEntry"/> selects out of <paramref name="entries"/>
    /// </summary>
    private static unsafe int SelectEntryIndex(TranspositionTableElement[] entries, ushort key, int currentAge)
    {
        Assert.AreEqual(Constants.TranspositionTableEntriesPerBucket, entries.Length);

        fixed (TranspositionTableElement* bucket = entries)
        {
            return (int)(TranspositionTable.SelectEntry(bucket, key, currentAge) - bucket);
        }
    }

    [Test]
    public void SelectEntry_SameKey_IsAlwaysSelected_EvenIfNotTheLeastValuable()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 10, age: 4),
            CreateEntry(key: 2, depth: 1, age: 4),      // Least valuable
            CreateEntry(key: 3, depth: 20, age: 4)
        };

        Assert.AreEqual(2, SelectEntryIndex(entries, key: 3, currentAge: 4));
        Assert.AreEqual(0, SelectEntryIndex(entries, key: 1, currentAge: 4));
        Assert.AreEqual(1, SelectEntryIndex(entries, key: 2, currentAge: 4));
    }

    [Test]
    public void SelectEntry_DifferentKey_SelectsLowestDepth()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 9, age: 4),
            CreateEntry(key: 2, depth: 3, age: 4),
            CreateEntry(key: 3, depth: 7, age: 4)
        };

        Assert.AreEqual(1, SelectEntryIndex(entries, key: 99, currentAge: 4));
    }

    [Test]
    public void SelectEntry_DifferentKey_PrefersStaticEvalOnlyEntriesOverShallowRealOnes()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 1, age: 4),
            CreateEntry(key: 2, depth: 0, age: 4, NodeType.Unknown),
            CreateEntry(key: 3, depth: 2, age: 4)
        };

        Assert.AreEqual(1, SelectEntryIndex(entries, key: 99, currentAge: 4));
    }

    [Test]
    public void SelectEntry_DifferentKey_OldDeeperEntryLosesToRecentShallowOnes()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 10, age: 1),     // 3 searches old: 10 - 2*3 = 4
            CreateEntry(key: 2, depth: 5, age: 4),      // 5
            CreateEntry(key: 3, depth: 6, age: 4)       // 6
        };

        Assert.AreEqual(0, SelectEntryIndex(entries, key: 99, currentAge: 4));
    }

    [Test]
    public void SelectEntry_DifferentKey_RelativeAgeIsCircular()
    {
        // Current age is 1: entry age 31 is 2 searches old, entry age 0 is 1 search old, entry age 1 is recent
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 10, age: 31),    // 10 - 2*2 = 6
            CreateEntry(key: 2, depth: 10, age: 0),     // 10 - 2*1 = 8
            CreateEntry(key: 3, depth: 10, age: 1)      // 10
        };

        Assert.AreEqual(0, SelectEntryIndex(entries, key: 99, currentAge: 1));
    }

    [Test]
    public void SelectEntry_DifferentKey_NeverWrittenEntriesAreSelectedFirst()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 1, age: 1),
            default,
            CreateEntry(key: 3, depth: 0, age: 1, NodeType.Unknown)
        };

        // Zeroed entries have age 0, i.e. they are 1 search old when the current age is 1
        Assert.AreEqual(1, SelectEntryIndex(entries, key: 99, currentAge: 1));
    }

    /// <summary>
    /// After the age wraps around, entries from the previous cycle have a higher relative age than zeroed ones, so by value alone they'd be selected first
    /// (i.e. current age 5: zeroed entries are worth 0 - 2*5 = -10, a depth 0 entry from age 31 is worth 0 - 2*6 = -12)
    /// </summary>
    [TestCase(0)]
    [TestCase(5)]
    [TestCase(20)]
    public void SelectEntry_DifferentKey_NeverWrittenEntriesAreSelectedFirst_EvenIfOlderEntriesHaveLowerValue(int currentAge)
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 0, age: 31),
            default,
            CreateEntry(key: 3, depth: 0, age: 31)
        };

        Assert.AreEqual(1, SelectEntryIndex(entries, key: 99, currentAge));
    }

    [Test]
    public void SelectEntry_DifferentKey_StaticEvalOnlyEntriesAreNotConsideredNeverWritten()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 0, age: 4, NodeType.Unknown),    // value: 0
            CreateEntry(key: 2, depth: 0, age: 1),                      // value: 0 - 2*3 = -6
            CreateEntry(key: 3, depth: 5, age: 4)
        };

        Assert.AreEqual(1, SelectEntryIndex(entries, key: 99, currentAge: 4));
    }

    [Test]
    public void SelectEntry_DifferentKey_TiesSelectTheFirstEntry()
    {
        var entries = new[]
        {
            CreateEntry(key: 1, depth: 5, age: 4),
            CreateEntry(key: 2, depth: 5, age: 4),
            CreateEntry(key: 3, depth: 5, age: 4)
        };

        Assert.AreEqual(0, SelectEntryIndex(entries, key: 99, currentAge: 4));
    }

    [Test]
    public unsafe void RecordHash_SamePositionTwice_ReusesTheEntry()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();
            var position = new Position(Constants.InitialPositionFEN);

            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 5, ply: 0, score: 10, NodeType.Beta, wasPv: false, move: 1234);
            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 9, ply: 0, score: 20, NodeType.Beta, wasPv: false, move: 1234);

            Assert.AreEqual(9, FindOnlyRecordedEntry(tt)->Depth);
            Assert.True(tt.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: 0, out var ttEntry));
            Assert.AreEqual(20, ttEntry.Score);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public unsafe void RecordHash_SameAge_ShallowerNonExactEntryDoesNotReplaceDeeperOne()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();
            var position = new Position(Constants.InitialPositionFEN);

            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 20, ply: 0, score: 10, NodeType.Beta, wasPv: false, move: 1234);
            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 2, ply: 0, score: 20, NodeType.Beta, wasPv: false, move: 1234);

            Assert.AreEqual(20, FindOnlyRecordedEntry(tt)->Depth);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public unsafe void RecordHash_EntryFromPreviousSearch_IsReplacedEvenByShallowerOne()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();
            var position = new Position(Constants.InitialPositionFEN);

            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 20, ply: 0, score: 10, NodeType.Beta, wasPv: false, move: 1234);
            tt.BumpAge();
            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 2, ply: 0, score: 20, NodeType.Beta, wasPv: false, move: 1234);

            var entry = FindOnlyRecordedEntry(tt);
            Assert.AreEqual(2, entry->Depth);
            Assert.AreEqual(1, entry->Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public unsafe void SaveStaticEval_AfterRecordHash_ReusesTheEntryOfTheSamePosition()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();
            var position = new Position(Constants.InitialPositionFEN);

            tt.SaveStaticEval(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 7, wasPv: false);
            tt.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 7, depth: 5, ply: 0, score: 10, NodeType.Exact, wasPv: false, move: 1234);

            var entry = FindOnlyRecordedEntry(tt);
            Assert.AreEqual(5, entry->Depth);
            Assert.AreEqual(NodeType.Exact, entry->Type);
        }
        finally
        {
            tt.Release();
        }
    }

    [TestCase(1, 333)]
    [TestCase(2, 666)]
    [TestCase(3, 1000)]
    public unsafe void HashfullPermillApprox_CountsEntriesNotBuckets(int populatedEntriesPerBucket, int expectedPermill)
    {
        Assert.AreEqual(3, Constants.TranspositionTableEntriesPerBucket, "Test written for 3 entries per bucket");

        var tt = new TranspositionTable();
        try
        {
            tt.Clear();
            Assert.AreEqual(0, tt.HashfullPermillApprox());

            for (int bucketIndex = 0; bucketIndex < 1_000; ++bucketIndex)
            {
                for (int entryIndex = 0; entryIndex < populatedEntriesPerBucket; ++entryIndex)
                {
                    tt.Get(bucketIndex, entryIndex)->Update(key: 1, score: 0, staticEval: 0, depth: 1, NodeType.Beta, wasPv: 0, move: null, age: 0);
                }
            }

            Assert.AreEqual(expectedPermill, tt.HashfullPermillApprox());
        }
        finally
        {
            tt.Release();
        }
    }

    private static unsafe TranspositionTableElement* FindOnlyRecordedEntry(TranspositionTable tt)
    {
        TranspositionTableElement* found = null;

        for (int bucketIndex = 0; bucketIndex < (int)tt.Length; ++bucketIndex)
        {
            for (int entryIndex = 0; entryIndex < Constants.TranspositionTableEntriesPerBucket; ++entryIndex)
            {
                var entry = tt.Get(bucketIndex, entryIndex);
                if (!entry->IsEmpty)
                {
                    Assert.IsTrue(found is null, "More than one entry recorded");
                    found = entry;
                }
            }
        }

        Assert.IsTrue(found is not null, "No entry recorded");
        return found!;
    }

    [Test]
    [Explicit]
    [NonParallelizable]
    [Category(Categories.LongRunning)]
    public unsafe void ClearTT()
    {
        Configuration.EngineSettings.TranspositionTableSize = 31;
        Configuration.EngineSettings.Threads = 7;

        var tt = new TranspositionTable();

        Assert.AreNotEqual(0, (int)tt.Length % Configuration.EngineSettings.Threads, "We want to test the edge case where the last thread clears more items than the rest");

        for (int index = 0; index < (int)tt.Length; ++index)
        {
            for (int entryIndex = 0; entryIndex < Constants.TranspositionTableEntriesPerBucket; ++entryIndex)
            {
                var ttEntry = tt.Get(index, entryIndex);
                ttEntry->Update(1, 2, 3, 4, NodeType.Exact, 5, 6, 7);
            }
        }

        tt.Clear();

        for (int index = 0; index < (int)tt.Length; ++index)
        {
            for (int entryIndex = 0; entryIndex < Constants.TranspositionTableEntriesPerBucket; ++entryIndex)
            {
                var ttEntry = tt.Get(index, entryIndex);

                Assert.AreEqual(0, ttEntry->Age);
                Assert.AreEqual(0, ttEntry->Score);
                Assert.AreEqual(0, ttEntry->StaticEval);
                Assert.AreEqual(0, ttEntry->Depth);
                Assert.AreEqual(NodeType.Unknown, ttEntry->Type);
                Assert.AreEqual(false, ttEntry->WasPv);
                Assert.AreEqual(0, ttEntry->Move);
                Assert.AreEqual(0, ttEntry->Key);
            }
        }
    }

    /// <summary>
    /// Ensures <see cref="SingleArrayTranspositionTable.HashfullPermillApprox"/> works correctly
    /// </summary>
    [Test]
    [Explicit]
    [NonParallelizable]
    public void TranspositionTableSize()
    {
        Configuration.EngineSettings.TranspositionTableSize = 1;
        var tt = new TranspositionTable();

        Assert.GreaterOrEqual(tt.Length, 1000);
    }
}