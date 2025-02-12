using Lynx.Model;

namespace Lynx;

public static class Masks
{
    /// <summary>
    /// File mask for square f2 (same one as f3, f4, etc.)
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
    /// 8  0 1 1 1 0 0 0 0
    /// 7  0 1 1 1 0 0 0 0
    /// 6  0 1 1 1 0 0 0 0
    /// 5  0 1 1 1 0 0 0 0
    /// 4  0 0 x 0 0 0 0 0
    /// 3  0 0 0 0 0 0 0 0
    /// 2  0 0 0 0 0 0 0 0
    /// 1  0 0 0 0 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] WhitePassedPawnMasks { get; } = new BitBoard[64];

    /// <summary>
    /// Passed pawn mask for square c5
    /// 8  0 0 0 0 0 0 0 0
    /// 7  0 0 0 0 0 0 0 0
    /// 6  0 0 0 0 0 0 0 0
    /// 5  0 0 x 0 0 0 0 0
    /// 4  0 1 1 1 0 0 0 0
    /// 3  0 1 1 1 0 0 0 0
    /// 2  0 1 1 1 0 0 0 0
    /// 1  0 1 1 1 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] BlackPassedPawnMasks { get; } = new BitBoard[64];

    /// <summary>
    /// Passed 'side' pawn mask for square c4
    /// 8  0 1 0 1 0 0 0 0
    /// 7  0 1 0 1 0 0 0 0
    /// 6  0 1 0 1 0 0 0 0
    /// 5  0 1 0 1 0 0 0 0
    /// 4  0 0 x 0 0 0 0 0
    /// 3  0 0 0 0 0 0 0 0
    /// 2  0 0 0 0 0 0 0 0
    /// 1  0 0 0 0 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] WhiteSidePassedPawnMasks { get; } = new BitBoard[64];

    /// <summary>
    /// 8   0 1 1 1 1 1 1 0
    /// 7   0 1 1 1 1 1 1 0
    /// 6   0 1 1 1 1 1 1 0
    /// 5   0 1 1 1 1 1 1 0
    /// 4   0 0 0 0 0 0 0 0
    /// 3   0 0 0 0 0 0 0 0
    /// 2   0 0 0 0 0 0 0 0
    /// 1   0 0 0 0 0 0 0 0
    ///     a b c d e f g h
    ///     Bitboard: 2122219134 (0x7E7E7E7E)
    /// </summary>
    public static BitBoard WhiteKnightOutpostMask { get; }

    /// <summary>
    /// 8   0 0 0 0 0 0 0 0
    /// 7   0 0 0 0 0 0 0 0
    /// 6   0 0 0 0 0 0 0 0
    /// 5   0 0 0 0 0 0 0 0
    /// 4   0 1 1 1 1 1 1 0
    /// 3   0 1 1 1 1 1 1 0
    /// 2   0 1 1 1 1 1 1 0
    /// 1   0 1 1 1 1 1 1 0
    ///     a b c d e f g h
    ///     Bitboard: 9114861775475441664 (0x7E7E7E7E00000000)
    /// </summary>
    public static BitBoard BlackKnightOutpostMask { get; }

    /// <summary>
    /// Passed 'side' pawn mask for square c5
    /// 8  0 0 0 0 0 0 0 0
    /// 7  0 0 0 0 0 0 0 0
    /// 6  0 0 0 0 0 0 0 0
    /// 5  0 0 x 0 0 0 0 0
    /// 4  0 1 0 1 0 0 0 0
    /// 3  0 1 0 1 0 0 0 0
    /// 2  0 1 0 1 0 0 0 0
    /// 1  0 1 0 1 0 0 0 0
    ///    a b c d e f g h
    /// </summary>
    public static BitBoard[] BlackSidePassedPawnMasks { get; } = new BitBoard[64];

    public static BitBoard[] WhiteBehindPawnMask { get; } = new BitBoard[64];

    public static BitBoard[] BlackBehindPawnMask { get; } = new BitBoard[64];

    public static readonly BitBoard[][] BehindPawnMask =
    [
        WhiteBehindPawnMask,
        [],
        [],
        [],
        [],
        [],
        BlackBehindPawnMask,
        [],
        [],
        [],
        [],
        [],
    ];

    /// <summary>
    /// __builtin_bswap64(<see cref="LightSquaresMask"/>)
    /// </summary>
    public const BitBoard DarkSquaresMask = 0x55AA_55AA_55AA_55AA;

    /// <summary>
    /// https://www.chessprogramming.org/Color_of_a_Square
    /// </summary>
    public const BitBoard LightSquaresMask = 0xAA55_AA55_AA55_AA55;

    static Masks()
    {
#pragma warning disable S3353, RCS1118 // Unchanged local variables should be "const" - FP https://community.sonarsource.com/t/fp-s3353-value-modified-in-ref-extension-method/132389
        ulong whiteKnightOutpostMask = 0, blackKnightOutpostMask = 0;
#pragma warning restore S3353, RCS1118 // Unchanged local variables should be "const"

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (rank < 4 && file > 0 && file < 7)
                {
                    whiteKnightOutpostMask.SetBit(squareIndex);
                    blackKnightOutpostMask.SetBit(squareIndex ^ 56);
                }

                FileMasks[squareIndex] |= SetFileRankMask(file, -1);
                RankMasks[squareIndex] |= SetFileRankMask(-1, rank);
                IsolatedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                IsolatedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file, -1);
                WhitePassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                WhiteSidePassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                WhiteSidePassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                // Reset bits behind the pawn
                for (int i = 7; i >= rank; --i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        WhitePassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                        WhiteSidePassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                    }

                    WhiteBehindPawnMask[squareIndex].SetBit(BitBoardExtensions.SquareIndex(i, file));
                }

                // Make sure the square doesn't count
                WhiteBehindPawnMask[squareIndex].PopBit(BitBoardExtensions.SquareIndex(rank, file));

                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file, -1);
                BlackPassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                BlackSidePassedPawnMasks[squareIndex] |= SetFileRankMask(file - 1, -1);
                BlackSidePassedPawnMasks[squareIndex] |= SetFileRankMask(file + 1, -1);

                // Reset bits behind the pawn
                for (int i = 0; i < rank + 1; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        BlackPassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                        BlackSidePassedPawnMasks[squareIndex].PopBit(BitBoardExtensions.SquareIndex(i, j));
                    }

                    BlackBehindPawnMask[squareIndex].SetBit(BitBoardExtensions.SquareIndex(i, file));
                }

                // Make sure the square doesn't count
                BlackBehindPawnMask[squareIndex].PopBit(BitBoardExtensions.SquareIndex(rank, file));
            }
        }

        WhiteKnightOutpostMask = whiteKnightOutpostMask;
        BlackKnightOutpostMask = blackKnightOutpostMask;
    }

#pragma warning disable S1066 // Collapsible "if" statements should be merged - init only code, clarity over speed here

    private static BitBoard SetFileRankMask(int fileIndex, int rankIndex)
    {
#pragma warning disable S3353 // Unchanged local variables should be "const" - FP https://community.sonarsource.com/t/fp-s3353-value-modified-in-ref-extension-method/132389
        BitBoard mask = 0;
#pragma warning restore S3353 // Unchanged local variables should be "const"

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

#pragma warning restore S1066 // Collapsible "if" statements should be merged
}
