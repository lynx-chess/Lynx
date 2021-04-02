using SharpFish.Model;
using System;

namespace SharpFish
{
    internal static class ConstantGenerators
    {
        public static ulong NotAFile()
        {
            BitBoard b = new();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    if (file > 0)
                    {
                        b.SetBit(squareIndex);
                    }
                }
            }

            b.Print();

            return b.Board;
        }

        public static ulong NotHFile()
        {
            BitBoard b = new();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    if (file < 7)
                    {
                        b.SetBit(squareIndex);
                    }
                }
            }

            b.Print();

            return b.Board;
        }

        public static ulong NotABFiles()
        {
            BitBoard b = new();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    if (file > 1)
                    {
                        b.SetBit(squareIndex);
                    }
                }
            }

            b.Print();

            return b.Board;
        }

        public static ulong NotHGFiles()
        {
            BitBoard b = new();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    if (file < 6)
                    {
                        b.SetBit(squareIndex);
                    }
                }
            }

            b.Print();

            return b.Board;
        }

        public static void PrintSquares()
        {
            for (int rank = 8; rank >= 1; --rank)
            {
                //Console.WriteLine($"a{rank}, b{rank}, c{rank}, d{rank}, e{rank}, f{rank}, g{rank}, h{rank},");
                Console.WriteLine($"\"a{rank}\", \"b{rank}\", \"c{rank}\", \"d{rank}\", \"e{rank}\", \"f{rank}\", \"g{rank}\", \"h{rank}\",");
            }
        }
    }
}
