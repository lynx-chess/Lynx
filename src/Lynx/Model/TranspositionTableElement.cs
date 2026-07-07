using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable S4022 // Enumerations should have "Int32" storage - size matters
public enum NodeType : byte
#pragma warning restore S4022 // Enumerations should have "Int32" storage
{
    Unknown,    // It needs to be 0 because of default struct initialization

    Exact,

    /// <summary>
    /// UpperBound
    /// </summary>
    Alpha,

    /// <summary>
    /// LowerBound
    /// </summary>
    Beta,
}

/// <summary>
/// 10 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TranspositionTableElement
{
    private ushort _key;        // 2 bytes

    private ShortMove _move;    // 2 bytes

    private short _score;       // 2 bytes

    private short _staticEval;  // 2 bytes

    private byte _depth;        // 1 byte

    /// <summary>
    /// 1 byte
    /// Binary move bits    Hexadecimal
    /// 0000 0001              0x1           Was PV (0-1)
    /// 0000 0110              0x6           NodeType (0-3)
    /// 1111 1000              0xF8          Age (0-32)
    /// </summary>
    private byte _type_WasPv;   // 1 byte

    private const int WasPvMask = 0x1;

    private const int NodeTypeOffset = 1;
    private const int NodeTypeMask = 0x6;

    /// <summary>
    /// Max age, 31
    /// </summary>
    public const int MaxAge = 0b11111;

    private const int AgeOffset = 3;
    internal const int AgeMask = 0xF8;

    /// <summary>
    /// 16 MSB of Position's Zobrist key
    /// </summary>
    public readonly ushort Key
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _key;
    }

    /// <summary>
    /// Best move found in the position. 0 if the search failed low (score <= alpha)
    /// </summary>
    public readonly ShortMove Move
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _move;
    }

    /// <summary>
    /// Position's score
    /// </summary>
    public readonly int Score
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _score;
    }

    /// <summary>
    /// Position's static evaluation
    /// </summary>
    public readonly int StaticEval
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _staticEval;
    }

    /// <summary>
    /// How deep the recorded search went. For us this numberis targetDepth - ply
    /// </summary>
    public readonly int Depth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _depth;
    }

    public readonly bool WasPv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_type_WasPv & WasPvMask) == 1;
    }

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public readonly NodeType Type
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (NodeType)((_type_WasPv & NodeTypeMask) >> NodeTypeOffset);
    }


    public readonly int Age
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_type_WasPv & AgeMask) >> AgeOffset;
    }

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ulong)Marshal.SizeOf<TranspositionTableElement>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(ushort key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, Move? move, int age)
    {
        Debug.Assert(age <= MaxAge);

        _key = key;
        _score = (short)score;
        _staticEval = (short)staticEval;
        _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
        _type_WasPv = (byte)(
            wasPv
            | ((int)nodeType << NodeTypeOffset)
            | (age << AgeOffset));
        _move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
    }
}
