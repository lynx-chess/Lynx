using Lynx.Model;

namespace Lynx;

public static class Masks
{
    /// <summary>
    /// File mask for square f2 (same one as f3, f4, etc.)
    ///
    ///  8  0 0 0 0 0 1 0 0
    ///  7  0 0 0 0 0 1 0 0
    ///  6  0 0 0 0 0 1 0 0
    ///  5  0 0 0 0 0 1 0 0
    ///  4  0 0 0 0 0 1 0 0
    ///  3  0 0 0 0 0 1 0 0
    ///  2  0 0 0 0 0 1 0 0
    ///  1  0 0 0 0 0 1 0 0
    ///     a b c d e f g h
    /// </summary>
    public static BitBoard[] FileMasks { get; } = new BitBoard[64];

    /// <summary>
    /// Rank mask for square a6 (same one for b6, c6, etc.)
    /// 
    /// 8  0 0 0 0 0 0 0 0
    /// 7  0 0 0 0 0 0 0 0
    /// 6  1 1 1 1 1 1 1 1
    /// 5  0 0 0 0 0 0 0 0
    /// 4  0 0 0 0 0 0 0 0
    /// 3  0 0 0 0 0 0 0 0
    /// 2  0 0 0 0 0 0 0 0
    /// 1  0 0 0 0 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] RankMasks { get; } = new BitBoard[64];

    /// <summary>
    /// Isolated mask for square g2 (same for g3, g4, etc.)
    ///   
    /// 8  0 0 0 0 0 1 0 1
    /// 7  0 0 0 0 0 1 0 1
    /// 6  0 0 0 0 0 1 0 1
    /// 5  0 0 0 0 0 1 0 1
    /// 4  0 0 0 0 0 1 0 1
    /// 3  0 0 0 0 0 1 0 1
    /// 2  0 0 0 0 0 1 0 1
    /// 1  0 0 0 0 0 1 0 1
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] IsolatedPawnMasks { get; } = new BitBoard[64];


    /// <summary>
    /// Passed pawn mask for square c4
    ///         
    /// 8  0 1 1 1 0 0 0 0
    /// 7  0 1 1 1 0 0 0 0
    /// 6  0 1 1 1 0 0 0 0
    /// 5  0 1 1 1 0 0 0 0
    /// 4  0 0 0 0 0 0 0 0
    /// 3  0 0 0 0 0 0 0 0
    /// 2  0 0 0 0 0 0 0 0
    /// 1  0 0 0 0 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] WhitePassedPawnMasks { get; } = new BitBoard[64];

    /// <summary>
    /// Passed pawn mask for square c5
    ///         
    /// 8  0 0 0 0 0 0 0 0
    /// 7  0 0 0 0 0 0 0 0
    /// 6  0 0 0 0 0 0 0 0
    /// 5  0 0 0 0 0 0 0 0
    /// 4  0 1 1 1 0 0 0 0
    /// 3  0 1 1 1 0 0 0 0
    /// 2  0 1 1 1 0 0 0 0
    /// 1  0 1 1 1 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] BlackPassedPawnMasks { get; } = new BitBoard[64];

    public static readonly BitBoard[][] PassedPawns = new BitBoard[12][]
    {
        WhitePassedPawnMasks,
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        BlackPassedPawnMasks,
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
        Array.Empty<BitBoard>(),
    };

    static Masks()
    {
        InitializeMasks();
    }

    private static void InitializeMasks()
    {
        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                FileMasks[squareIndex] |= SetFileRankMask(file, -1);
                RankMasks[squareIndex] |= SetFileRankMask(-1, rank);
                IsolatedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                IsolatedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file, -1);
                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                // Reset bits behind the pawn
                for (int i = 7; i >= rank; --i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        WhitePassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                    }
                }

                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file, -1);
                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                // Reset bits behind the pawn
                for (int i = 0; i < rank + 1; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        BlackPassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                    }
                }
            }
        }
    }

    private static BitBoard SetFileRankMask(int fileIndex, int rankIndex)
    {
        BitBoard mask = 0;

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (fileIndex != -1)
                {
                    if (file == fileIndex)
                    {
                        mask.SetBit(squareIndex);
                    }
                }

                else if (rankIndex != -1)
                {
                    if (rank == rankIndex)
                    {
                        mask.SetBit(squareIndex);
                    }
                }
            }
        }

        return mask;
    }
}
