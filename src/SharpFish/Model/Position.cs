using System;

namespace SharpFish.Model
{
    public readonly struct Position
    {
        /// <summary>
        /// Use <see cref="Piece"/> as index
        /// </summary>
        public BitBoard[] PieceBitBoards { get; }

        /// <summary>
        /// Black, White, Both
        /// </summary>
        public BitBoard[] OccupancyBitBoards { get; }

        public Side Side { get; }

        public BoardSquares EnPassant { get; }

        public int Castle { get; }

        public Position(string fen)
        {
            var parsedFEN = FENParser.ParseFEN(fen);

            if (!parsedFEN.Success)
            {
                Console.WriteLine($"Error parsing FEN {fen}");
            }

            PieceBitBoards = parsedFEN.PieceBitBoards;
            OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
            Side = parsedFEN.Side;
            Castle = parsedFEN.Castle;
            EnPassant = parsedFEN.EnPassant;
        }

        /// <summary>
        /// Combines <see cref="PieceBitBoards"/>, <see cref="Side"/>, <see cref="Castle"/> and <see cref="EnPassant"/>
        /// into a human-friendly representation
        /// </summary>
        public readonly void Print()
        {
            const string separator = "____________________________________________________";
            Console.WriteLine(separator);

            for (var rank = 0; rank < 8; ++rank)
            {
                for (var file = 0; file < 8; ++file)
                {
                    if (file == 0)
                    {
                        Console.Write($"{8 - rank}  ");
                    }

                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    var piece = -1;

                    for (int bbIndex = 0; bbIndex < PieceBitBoards.Length; ++bbIndex)
                    {
                        if (PieceBitBoards[bbIndex].GetBit(squareIndex))
                        {
                            piece = bbIndex;
                        }
                    }

                    var pieceRepresentation = piece == -1
                        ? '.'
                        : Constants.AsciiPieces[piece];

                    Console.Write($" {pieceRepresentation}");
                }

                Console.WriteLine();
            }

            Console.Write("\n    a b c d e f g h\n");

            Console.WriteLine();
            Console.WriteLine($"    Side:\t{Side}");
            Console.WriteLine($"    Enpassant:\t{(EnPassant == BoardSquares.noSquare ? "no" : Constants.Coordinates[(int)EnPassant])}");
            Console.WriteLine($"    Castling:\t" +
                $"{((Castle & (int)CastlingRights.WK) != default ? 'K' : '-')}" +
                $"{((Castle & (int)CastlingRights.WQ) != default ? 'Q' : '-')} | " +
                $"{((Castle & (int)CastlingRights.BK) != default ? 'k' : '-')}" +
                $"{((Castle & (int)CastlingRights.BQ) != default ? 'q' : '-')}"
                );

            Console.WriteLine(separator);
        }

        public readonly void PrintAttackedSquares(Side sideToMove)
        {
            const string separator = "____________________________________________________";
            Console.WriteLine(separator);

            for (var rank = 0; rank < 8; ++rank)
            {
                for (var file = 0; file < 8; ++file)
                {
                    if (file == 0)
                    {
                        Console.Write($"{8 - rank}  ");
                    }

                    var squareIndex = BitBoard.SquareIndex(rank, file);

                    var pieceRepresentation = Attacks.IsSquaredAttacked(squareIndex, sideToMove, PieceBitBoards, OccupancyBitBoards)
                        ? '1'
                        : '.';

                    Console.Write($" {pieceRepresentation}");
                }

                Console.WriteLine();
            }

            Console.Write("\n    a b c d e f g h\n");
            Console.WriteLine(separator);
        }
    }
}
