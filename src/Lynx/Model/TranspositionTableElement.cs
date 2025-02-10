using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable S4022 // Enumerations should have "Int32" storage - size matters
public enum NodeType : byte
#pragma warning restore S4022 // Enumerations should have "Int32" storage
{
    Unknown,    // Making it 0 instead of -1 because of default struct initialization

    Exact,

    /// <summary>
    /// UpperBound
    /// </summary>
    Alpha,

    /// <summary>
    /// LowerBound
    /// </summary>
    Beta
}

/// <summary>
/// 10 bytes
/// </summary>
public struct TranspositionTableElement
{

    private ushort _key;            // 2 bytes

    private ShortMove _move;        // 2 bytes

    private short _score;           // 2 bytes

    private short _staticEval;      // 2 bytes

    private byte _depth;            // 1 byte

    /// <summary>
    /// 1 byte
    /// Binary bits        Hexadecimal
    /// 0001 1111              0x1F         Age (0-53)
    /// 0010 0000              0x20         Was PV (0-1)
    /// 1100 0000              0xC0         NodeType (0-3)
    /// </summary>
    private byte _typeWasPvAge;   // 1 byte

    public const int AgeBitCount = 5;
    public const int NodeTypeOffset = 6;
    public const int WasPvOffset = 5;

    /// <summary>
    /// 16 MSB of Position's Zobrist key
    /// </summary>
    public readonly ushort Key => _key;

    /// <summary>
    /// Best move found in the position. 0 if the search failed low (score <= alpha)
    /// </summary>
    public readonly ShortMove Move => _move;

    /// <summary>
    /// Position's score
    /// </summary>
    public readonly int Score => _score;

    /// <summary>
    /// Position's static evaluation
    /// </summary>
    public readonly int StaticEval => _staticEval;

    /// <summary>
    /// How deep the recorded search went. For us this numberis targetDepth - ply
    /// </summary>
    public readonly int Depth => _depth;

    public readonly int Age => _typeWasPvAge & 0x1F;

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public readonly NodeType Type => (NodeType)((_typeWasPvAge & 0xC0) >> NodeTypeOffset);

    public readonly bool WasPv => ((_typeWasPvAge & 0x20) >> WasPvOffset) == 1;

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size => (ulong)Marshal.SizeOf<TranspositionTableElement>();

    public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, int ttAge, Move? move)
    {
        Debug.Assert(ttAge < (1 << AgeBitCount));

        _key = (ushort)key;
        _score = (short)score;
        _staticEval = (short)staticEval;
        _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
        _move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
        _typeWasPvAge = (byte)(
            ttAge
            | (wasPv << WasPvOffset)
            | ((int)nodeType << NodeTypeOffset));
    }
}
