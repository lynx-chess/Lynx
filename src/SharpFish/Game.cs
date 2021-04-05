using SharpFish.Model;
using System;
using System.Runtime.InteropServices;

namespace SharpFish
{
    public class Game
    {
        private readonly BitBoard[] _bishopOccupancyMasks;
        private readonly BitBoard[] _rookOccupancyMasks;

        /// <summary>
        /// [2 (W/B), 64 (Squares)]
        /// </summary>
        private readonly BitBoard[,] _pawnAttacks;
        private readonly BitBoard[] _knightAttacks;
        private readonly BitBoard[] _kingAttacks;

        /// <summary>
        /// [64 (Squares), 512 (Occupancies)]
        /// Use <see cref="GetBishopAttacks(int, BitBoard)"/>
        /// </summary>
        private readonly BitBoard[,] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="GetRookAttacks(int, BitBoard)"/>
        /// </summary>
        private readonly BitBoard[,] _rookAttacks;

        //private readonly BitBoard[] PieceBitBoards = new BitBoard[12];

        /// <summary>
        /// Black, White, Both
        /// </summary>
        public BitBoard[] OccupancyBitBoards { get; } = new BitBoard[3];

        public BitBoard[] PieceBitBoards { get; } = new BitBoard[12];

        public Side Side { get; internal set; } = Side.Both;

        public BoardSquares EnPassant { get; internal set; } = BoardSquares.noSquare;

        public int Castle { get; internal set; }

        public Game()
        {
            _kingAttacks = AttacksGenerator.InitializeKingAttacks();
            _pawnAttacks = AttacksGenerator.InitializePawnAttacks();
            _knightAttacks = AttacksGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttacksGenerator.InitializeBishopAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttacksGenerator.InitializeRookAttacks();
        }

        public bool ParseFEN(string fen)
        {
            bool parseResultError;

            (parseResultError, Side, Castle, EnPassant, _, _) =
                FENParser.ParseFEN(fen, PieceBitBoards, OccupancyBitBoards);

            return parseResultError;
        }

        /// <summary>
        /// Get Bishop attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public BitBoard GetBishopAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _bishopOccupancyMasks[squareIndex].Board;
            occ *= Constants.BishopMagicNumbers[squareIndex];
            occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

            return _bishopAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Rook attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public BitBoard GetRookAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _rookOccupancyMasks[squareIndex].Board;
            occ *= Constants.RookMagicNumbers[squareIndex];
            occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

            return _rookAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Combines <see cref="PieceBitBoards"/>, <see cref="Side"/>, <see cref="Castle"/> and <see cref="EnPassant"/>
        /// into a human-friendly representation
        /// </summary>
        public void PrintBoard()
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

                    //for (int bb = (int)Piece.P; bb <= (int)Piece.k; ++bb)
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
                    //:(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    //    ? Constants.UnicodePieces[piece][0]
                    //    : Constants.AsciiPieces[piece]);

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
    }
}
