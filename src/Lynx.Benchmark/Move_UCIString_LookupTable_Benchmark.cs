/*
 * Pre-computed lookup table vs stackalloc+Span.ToString() vs Dictionary memoization
 *
 * The lookup table uses the lower 16 bits of a Move (promoted piece + source square + target square)
 * as an index into a string[65536], pre-populated at startup. This eliminates all runtime allocation
 * and computation, turning UCIString into a single array access.
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Benchmark;

public class Move_UCIString_LookupTable_Benchmark : BaseBenchmark
{
    private static readonly Move[] _moves =
    [
        MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K),
        MoveExtensions.EncodeLongCastle(Constants.InitialBlackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k),
        MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.p, promotedPiece: (int)Piece.q),
        MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.b8, (int)Piece.p, promotedPiece: (int)Piece.n, capturedPiece: (int)Piece.B),
        MoveExtensions.EncodeCapture((int)BoardSquare.a8, (int)BoardSquare.h1, (int)Piece.B, capturedPiece: (int)Piece.b),
        MoveExtensions.EncodeEnPassant((int)BoardSquare.e5, (int)BoardSquare.d6, (int)Piece.P),
        MoveExtensions.Encode((int)BoardSquare.g1, (int)BoardSquare.f3, (int)Piece.N),
        MoveExtensions.EncodeDoublePawnPush((int)BoardSquare.d2, (int)BoardSquare.d4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.Q),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.R),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.B),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.N),
    ];

    [Params(1, 10, 100, 1000)]
    public int Iterations { get; set; }

    [Benchmark(Baseline = true)]
    public StringBuilder StackallocSpan()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.StackallocUCIString());
            }
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder DictionaryMemoized()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.DictionaryMemoizedUCIString());
            }
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder LookupTable()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.LookupTableUCIString());
            }
        }

        return sb;
    }
}

file static class LookupBenchmarkHelpers
{
    private const int SourceSquareOffset = 4;
    private const int TargetSquareOffset = 10;

    [SkipLocalsInit]
    public static string StackallocUCIString(this Move move)
    {
        Span<char> span = stackalloc char[5];

        var source = Constants.CoordinatesCharArray[move.SourceSquare()];
        var target = Constants.CoordinatesCharArray[move.TargetSquare()];

        span[0] = source[0];
        span[1] = source[1];
        span[2] = target[0];
        span[3] = target[1];

        var promotedPiece = move.PromotedPiece();
        if (promotedPiece != default)
        {
            span[4] = Constants.AsciiPiecesLowercase[promotedPiece];

            return span.ToString();
        }

        return span[..^1].ToString();
    }

    private static readonly Dictionary<int, string> _cache = new(4096);

    public static string DictionaryMemoizedUCIString(this Move move)
    {
        if (_cache.TryGetValue(move, out var uciString))
        {
            return uciString;
        }

        var str = move.StackallocUCIString();
        _cache[move] = str;

        return str;
    }

    private static readonly string[] _uciStrings = InitUCIStrings();

    private static string[] InitUCIStrings()
    {
        var result = new string[ushort.MaxValue + 1];

        for (int source = 0; source < 64; source++)
        {
            for (int target = 0; target < 64; target++)
            {
                int baseIndex = (source << SourceSquareOffset) | (target << TargetSquareOffset);
                var baseStr = string.Concat(Constants.Coordinates[source], Constants.Coordinates[target]);
                result[baseIndex] = baseStr;

                for (int promotedPiece = 1; promotedPiece < 12; promotedPiece++)
                {
                    result[baseIndex | promotedPiece] = $"{baseStr}{Constants.AsciiPiecesLowercase[promotedPiece]}";
                }
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LookupTableUCIString(this Move move) => _uciStrings[move & 0xFFFF];
}
