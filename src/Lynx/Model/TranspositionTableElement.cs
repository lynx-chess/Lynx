﻿using System.Runtime.InteropServices;

namespace Lynx.Model;

public enum NodeType : byte
{
    Unknown,    // Making it 0 instead of -1 because of default struct initialization
    Exact,
    Alpha,
    Beta
}

/// <summary>
/// 10 bytes
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct TranspositionTableElement
{
    [FieldOffset(0)]
    private ushort _key;        // 2 bytes

    [FieldOffset(2)]
    private ShortMove _move;    // 2 bytes

    [FieldOffset(4)]
    private short _score;       // 2 bytes

    [FieldOffset(6)]
    private short _staticEval;  // 2 bytes

    [FieldOffset(8)]
    private byte _depth;        // 1 byte

    [FieldOffset(9)]
    private NodeType _type;     // 1 byte

    [FieldOffset(2)]
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 255)]
    private fixed byte _data[8];

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

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public readonly NodeType Type => _type;

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size => 10;// (ulong)Marshal.SizeOf<TranspositionTableElement>();

    public readonly int Data()
    {
        var result = _data[0] + _data[1] + _data[2] + _data[3] + _data[4] +_data[5] +_data[6] +_data[7];

        return result;
    }

    public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, Move? move)
    {
        _key = (ushort)key;
        _score = (short)score;
        _staticEval = (short)staticEval;
        _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
        _type = nodeType;
        _move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
    }
}
