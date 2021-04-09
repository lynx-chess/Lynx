using BenchmarkDotNet.Attributes;
using SharpFish.Model;
using System.Collections.Generic;

namespace SharpFish.Benchmark
{
    internal static class EncodeMoveImplementation
    {
        public static int EncodeMoveBool(int sourceSquare, int targetSquare, int piece, int promotedPiece = 0, bool isCapture = false, bool isDoublePawnPush = false, bool enPassant = false, bool isCastle = false)
        {
            var encodedMove = sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16);

            if (isCapture)
            {
                encodedMove |= (1 << 20);
            }

            if (isDoublePawnPush)
            {
                encodedMove |= (1 << 21);
            }

            if (enPassant)
            {
                encodedMove |= (1 << 22);
            }

            if (isCastle)
            {
                encodedMove |= (1 << 23);
            }

            return encodedMove;
        }

        public static int EncodeMoveInt(int sourceSquare, int targetSquare, int piece, int promotedPiece = default, int isCapture = default, int isDoublePawnPush = default, int enPassant = default, int isCastle = 0)
        {
            return sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
                | (isCapture << 20)
                | (isDoublePawnPush << 21)
                | (enPassant << 22)
                | (isCastle << 23);
        }
    }

    public class EncodeMove : BaseBenchmark
    {
        public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void EncodeMoveBool(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.a1, (int)BoardSquares.h8, (int)Piece.q);
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.a7, (int)BoardSquares.a8, (int)Piece.K, promotedPiece: (int)Piece.N);
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.a7, (int)BoardSquares.b8, (int)Piece.K, promotedPiece: (int)Piece.N, isCapture: true);
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.e2, (int)BoardSquares.e4, (int)Piece.K, isDoublePawnPush: true);
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.c7, (int)BoardSquares.b6, (int)Piece.K, enPassant: true, isCapture: true);
                EncodeMoveImplementation.EncodeMoveBool((int)BoardSquares.e8, (int)BoardSquares.g8, (int)Piece.k, isCastle: true);
            }
        }

        /// <summary>
        /// ~70x faster
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void EncodeMoveInt(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.a1, (int)BoardSquares.h8, (int)Piece.q);
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.a7, (int)BoardSquares.a8, (int)Piece.K, promotedPiece: (int)Piece.N);
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.a7, (int)BoardSquares.b8, (int)Piece.K, promotedPiece: (int)Piece.N, isCapture: 1);
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.e2, (int)BoardSquares.e4, (int)Piece.K, isDoublePawnPush: 1);
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.c7, (int)BoardSquares.b6, (int)Piece.K, enPassant: 1, isCapture: 1);
                EncodeMoveImplementation.EncodeMoveInt((int)BoardSquares.e8, (int)BoardSquares.g8, (int)Piece.k, isCastle: 1);
            }
        }
    }
}
