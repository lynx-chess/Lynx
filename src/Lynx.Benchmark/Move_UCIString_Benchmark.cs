using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Text;

namespace Lynx.Benchmark;
public class Move_UCIString_Benchmark : BaseBenchmark
{
    private static readonly Move[] _moves =
    [
        MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K),
        MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k),
        MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.p, promotedPiece: (int)Piece.q),
        MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.b8, (int)Piece.p, promotedPiece: (int)Piece.n, capturedPiece: (int)Piece.B),
        MoveExtensions.EncodeCapture((int)BoardSquare.a8, (int)BoardSquare.h1, (int)Piece.B, capturedPiece: (int)Piece.b),
        MoveExtensions.EncodeCapture((int)BoardSquare.a8, (int)BoardSquare.h1, (int)Piece.B),
        MoveExtensions.EncodeEnPassant((int)BoardSquare.e5, (int)BoardSquare.d6, (int)Piece.P)
    ];

    [Benchmark(Baseline = true)]
    public StringBuilder NaiveUCIString()
    {
        var sb = new StringBuilder();
        foreach (var move in _moves)
        {
            sb.Append(move.NaiveUCIString());
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder SpanUCIString()
    {
        var sb = new StringBuilder();
        foreach (var move in _moves)
        {
            sb.Append(move.SpanUCIString());
        }

        return sb;
    }
}

file static class MoveHelpers
{
    public static string NaiveUCIString(this Move move)
    {
        return
            Constants.Coordinates[move.SourceSquare()] +
            Constants.Coordinates[move.TargetSquare()] +
            (move.PromotedPiece() == default ? "" : $"{Constants.AsciiPieces[move.PromotedPiece()].ToString().ToLowerInvariant()}");
    }

    public static string SpanUCIString(this Move move)
    {
        Span<char> span = stackalloc char[5];

        var source = Constants.CoordinatesCharArray[move.SourceSquare()];
        span[0] = source[0];
        span[1] = source[1];

        var target = Constants.CoordinatesCharArray[move.TargetSquare()];
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
}
