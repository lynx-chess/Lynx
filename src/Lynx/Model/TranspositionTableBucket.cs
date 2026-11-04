using System.Runtime.InteropServices;

namespace Lynx.Model;

/// <summary>
/// <see cref="Constants.TranspositionTableEntriesPerBucket"/> consecutive <see cref="TranspositionTableElement"/>s,
/// padded to <see cref="Constants.TranspositionTableBucketSize"/> bytes.
/// A TranspositionTableBucket* can be cast to a TranspositionTableElement*, which can be indexed from 0 to
/// <see cref="Constants.TranspositionTableEntriesPerBucket"/> - 1 to access each entry.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = Constants.TranspositionTableBucketSize)]
public struct TranspositionTableBucket
{
#pragma warning disable S1144, RCS1213 // Unused private fields - just for visualization purposes, entries are accessed directly by casting a pointer to the bucket
    private TranspositionTableElement _entry0;      // 10 bytes

    private TranspositionTableElement _entry1;      // 10 bytes

    private TranspositionTableElement _entry2;      // 10 bytes

    private short _padding;                         // 2 bytes
#pragma warning restore S1144, RCS1213
}
