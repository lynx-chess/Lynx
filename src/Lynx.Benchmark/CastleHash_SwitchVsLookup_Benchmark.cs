using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class CastleHash_SwitchVsLookup_Benchmark : BaseBenchmark
{
    private static readonly ulong _WK_Hash = ZobristTable.PieceHash((int)BoardSquare.a8, (int)Piece.p);
    private static readonly ulong _WQ_Hash = ZobristTable.PieceHash((int)BoardSquare.b8, (int)Piece.p);
    private static readonly ulong _BK_Hash = ZobristTable.PieceHash((int)BoardSquare.c8, (int)Piece.p);
    private static readonly ulong _BQ_Hash = ZobristTable.PieceHash((int)BoardSquare.d8, (int)Piece.p);

    private byte[] _castlingRights = [];

    [Params(4_096, 65_536)]
    public int DataSize;

    [GlobalSetup]
    public void Setup()
    {
        _castlingRights = GC.AllocateArray<byte>(DataSize, pinned: true);

        var random = new Random(1234);
        for (int i = 0; i < _castlingRights.Length; ++i)
        {
            _castlingRights[i] = (byte)random.Next(0, 16);
        }
    }

    [Benchmark(Baseline = true)]
    public ulong CastleHash_Switch()
    {
        ulong hash = 0;

        for (int i = 0; i < _castlingRights.Length; ++i)
        {
            hash ^= CastleHashSwitch(_castlingRights[i]);
        }

        return hash;
    }

    [Benchmark]
    public ulong CastleHash_Lookup()
    {
        ulong hash = 0;

        for (int i = 0; i < _castlingRights.Length; ++i)
        {
            hash ^= ZobristTable.CastleHash(_castlingRights[i]);
        }

        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong CastleHashSwitch(byte castle)
    {
        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK => _WK_Hash,    // K    | -
            (byte)CastlingRights.WQ => _WQ_Hash,    // Q    | -
            (byte)CastlingRights.BK => _BK_Hash,    // -    | k
            (byte)CastlingRights.BQ => _BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => _WK_Hash ^ _WQ_Hash,    // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => _WK_Hash ^ _BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => _WK_Hash ^ _BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => _WQ_Hash ^ _BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => _WQ_Hash ^ _BQ_Hash,    // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _BK_Hash ^ _BQ_Hash,    // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => _WK_Hash ^ _WQ_Hash ^ _BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => _WK_Hash ^ _WQ_Hash ^ _BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _WK_Hash ^ _BK_Hash ^ _BQ_Hash,    // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _WQ_Hash ^ _BK_Hash ^ _BQ_Hash,    // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                _WK_Hash ^ _WQ_Hash ^ _BK_Hash ^ _BQ_Hash,

            _ => throw new LynxException($"Unexpected castle encoded number: {castle}")
        };
    }
}
