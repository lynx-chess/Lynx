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

    /// <summary>
    /// i.e. when storing only static evaluation
    /// </summary>
    None
}

/// <summary>
/// 10 bytes
/// </summary>
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
    /// 0000 1110              0xE           NodeType (0-4)
    /// 1111 0000              0xE           NodeType (0-4)
    /// </summary>
    private byte _age_type_WasPv;

    private const int NodeTypeOffset = 1;
    private const int NodeTypeMask = 0xE;

    private const int AgeOffset = 4;

    /// <summary>
    /// Max age, 15
    /// </summary>
    public const int MaxAge = 0xF0;
    public const int AgeMask = MaxAge - 1 ;

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

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public readonly NodeType Type
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (NodeType)((_age_type_WasPv & NodeTypeMask) >> NodeTypeOffset);
    }

    public readonly bool WasPv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_age_type_WasPv & 0x1) == 1;
    }

    public readonly int Age
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_age_type_WasPv /*& AgeExtractionMask*/) >> AgeOffset;
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
    public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, Move? move, int age)
    {
        Debug.Assert(age <= MaxAge);
        Debug.Assert(nodeType != NodeType.Unknown);

        _key = (ushort)key;
        _score = (short)score;
        _staticEval = (short)staticEval;
        _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
        _age_type_WasPv = (byte)(wasPv | ((int)nodeType << NodeTypeOffset) | age << AgeOffset);
        _move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
    }
}
