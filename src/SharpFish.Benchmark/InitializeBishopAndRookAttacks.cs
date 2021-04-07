using BenchmarkDotNet.Attributes;
using SharpFish.Model;

namespace SharpFish.Benchmark
{
    public class InitializeBishopAndRookAttacks : BaseBenchmark
    {
        [Benchmark(Baseline = true)]
        public void CurrentApproach() => new CustomGame();

        [Benchmark]
        public void CurrentApproachReusingLoop() => new CustomGame("");

        /// <summary>
        /// As Proposed in https://www.youtube.com/watch?v=1lAM8ffBg0A&list=PLmN0neTso3Jxh8ZIylk74JpwfiWNI76Cs&index=16
        /// </summary>
        [Benchmark]
        public void InitialApproach() => new CustomGame(0);

        public class CustomGame
        {
            private readonly BitBoard[,] _pawnAttacks = new BitBoard[2, 64];
            private readonly BitBoard[] _knightAttacks = new BitBoard[64];
            private readonly BitBoard[] _kingAttacks = new BitBoard[64];

            private readonly BitBoard[] _bishopOccupancyMasks = new BitBoard[64];
            private readonly BitBoard[] _rookOccupancyMasks = new BitBoard[64];

            private readonly BitBoard[,] _bishopAttacks = new BitBoard[64, 512];
            private readonly BitBoard[,] _rookAttacks = new BitBoard[64, 4096];

            public CustomGame()
            {
                _kingAttacks = AttacksGenerator.InitializeKingAttacks();
                _pawnAttacks = AttacksGenerator.InitializePawnAttacks();
                _knightAttacks = AttacksGenerator.InitializeKnightAttacks();

                (_bishopOccupancyMasks, _bishopAttacks) = AttacksGenerator.InitializeBishopAttacks();
                (_rookOccupancyMasks, _rookAttacks) = AttacksGenerator.InitializeRookAttacks();
            }

            /// <summary>
            /// Current reusing squares loop
            /// </summary>
            /// <param name="_"></param>
            public CustomGame(string _)
            {
                InitializePawnKnightAndKingAttacks();

                (_bishopOccupancyMasks, _bishopAttacks) = AttacksGenerator.InitializeBishopAttacks();
                (_rookOccupancyMasks, _rookAttacks) = AttacksGenerator.InitializeRookAttacks();
            }

            /// <summary>
            /// Initial approach
            /// </summary>
            /// <param name="_"></param>
            public CustomGame(int _)
            {
                InitializePawnKnightAndKingAttacks();

                InitializeRookAndBishopAttacks(isBishop: false);
                InitializeRookAndBishopAttacks(isBishop: true);
            }

            private void InitializePawnKnightAndKingAttacks()
            {
                for (int square = 0; square < 64; ++square)
                {
                    _pawnAttacks[0, square] = AttacksGenerator.MaskPawnAttacks(square, isWhite: false);
                    _pawnAttacks[1, square] = AttacksGenerator.MaskPawnAttacks(square, isWhite: true);

                    _knightAttacks[square] = AttacksGenerator.MaskKnightAttacks(square);

                    _kingAttacks[square] = AttacksGenerator.MaskKingAttacks(square);
                }
            }

            private void InitializeRookAndBishopAttacks(bool isBishop)
            {
                for (int square = 0; square < 64; ++square)
                {
                    _bishopOccupancyMasks[square] = AttacksGenerator.MaskBishopOccupancy(square);
                    _rookOccupancyMasks[square] = AttacksGenerator.MaskRookOccupancy(square);

                    var attackMask = isBishop
                                        ? _bishopOccupancyMasks[square]
                                        : _rookOccupancyMasks[square];

                    int relevantBitsCount = isBishop    // Or attackMask.CountBits()
                        ? Constants.BishopRelevantOccupancyBits[square]
                        : Constants.RookRelevantOccupancyBits[square];
                    int occupancyIndexes = (1 << relevantBitsCount);    // or Math.Pow(2, relevantBitsCount)

                    for (int index = 0; index < occupancyIndexes; ++index)
                    {
                        if (isBishop)
                        {
                            var occupancy = AttacksGenerator.SetBishopOrRookOccupancy(index, attackMask);

                            var magicIndex = (occupancy.Board * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                            _bishopAttacks[square, magicIndex] = AttacksGenerator.GenerateBishopAttacksOnTheFly(square, occupancy);
                        }
                        else
                        {
                            var occupancy = AttacksGenerator.SetBishopOrRookOccupancy(index, attackMask);

                            var magicIndex = (occupancy.Board * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                            _rookAttacks[square, magicIndex] = AttacksGenerator.GenerateRookAttacksOnTheFly(square, occupancy);
                        }
                    }
                }
            }
        }
    }
}
