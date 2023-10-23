using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class InitializeBishopAndRookAttacks : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public void CurrentApproach() => new CustomPosition();

    [Benchmark]
    public void CurrentApproachReusingLoop() => new CustomPosition("");

    /// <summary>
    /// As Proposed in https://www.youtube.com/watch?v=1lAM8ffBg0A&list=PLmN0neTso3Jxh8ZIylk74JpwfiWNI76Cs&index=16
    /// </summary>
    [Benchmark]
    public void InitialApproach() => new CustomPosition(0);

    public class CustomPosition
    {
        private readonly BitBoard[,] _pawnAttacks = new BitBoard[2, 64];
        private readonly BitBoard[] _knightAttacks = new BitBoard[64];
        private readonly BitBoard[] _kingAttacks = new BitBoard[64];

        private readonly BitBoard[] _bishopOccupancyMasks = new BitBoard[64];
        private readonly BitBoard[] _rookOccupancyMasks = new BitBoard[64];

        private readonly BitBoard[,] _bishopAttacks = new BitBoard[64, 512];
        private readonly BitBoard[,] _rookAttacks = new BitBoard[64, 4096];

        public CustomPosition()
        {
            _kingAttacks = AttackGenerator.InitializeKingAttacks();
            _pawnAttacks = AttackGenerator.InitializePawnAttacks();
            _knightAttacks = AttackGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();
        }

        /// <summary>
        /// Current reusing squares loop
        /// </summary>
        /// <param name="_"></param>
        public CustomPosition(string _)
        {
            InitializePawnKnightAndKingAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();
        }

        /// <summary>
        /// Initial approach
        /// </summary>
        /// <param name="_"></param>
        public CustomPosition(int _)
        {
            InitializePawnKnightAndKingAttacks();

            InitializeRookAndBishopAttacks(isBishop: false);
            InitializeRookAndBishopAttacks(isBishop: true);
        }

        private void InitializePawnKnightAndKingAttacks()
        {
            for (int square = 0; square < 64; ++square)
            {
                _pawnAttacks[0, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false);
                _pawnAttacks[1, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: true);

                _knightAttacks[square] = AttackGenerator.MaskKnightAttacks(square);

                _kingAttacks[square] = AttackGenerator.MaskKingAttacks(square);
            }
        }

        private void InitializeRookAndBishopAttacks(bool isBishop)
        {
            for (int square = 0; square < 64; ++square)
            {
                _bishopOccupancyMasks[square] = AttackGenerator.MaskBishopOccupancy(square);
                _rookOccupancyMasks[square] = AttackGenerator.MaskRookOccupancy(square);

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
                        var occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, attackMask);

                        var magicIndex = (occupancy * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                        _bishopAttacks[square, magicIndex] = AttackGenerator.GenerateBishopAttacksOnTheFly(square, occupancy);
                    }
                    else
                    {
                        var occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, attackMask);

                        var magicIndex = (occupancy * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                        _rookAttacks[square, magicIndex] = AttackGenerator.GenerateRookAttacksOnTheFly(square, occupancy);
                    }
                }
            }
        }
    }
}
