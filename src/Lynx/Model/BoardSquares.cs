namespace Lynx.Model
{
    /// <summary>
    /// Big-Endian Rank-File Mapping
    /// (opposte to LERF, https://www.chessprogramming.org/Square_Mapping_Considerations#LittleEndianRankFileMapping)
    ///
    ///         NW       N      NE
    ///           -9    -8    -7
    ///               \  |  /
    ///   W       -1 <-  0 -> +1    E
    ///               /  |  \
    ///           +7    +8    +9
    ///         SW       S      SE
    ///
    /// </summary>
    public enum BoardSquares
    {
        a8, b8, c8, d8, e8, f8, g8, h8,
        a7, b7, c7, d7, e7, f7, g7, h7,
        a6, b6, c6, d6, e6, f6, g6, h6,
        a5, b5, c5, d5, e5, f5, g5, h5,
        a4, b4, c4, d4, e4, f4, g4, h4,
        a3, b3, c3, d3, e3, f3, g3, h3,
        a2, b2, c2, d2, e2, f2, g2, h2,
        a1, b1, c1, d1, e1, f1, g1, h1,
        noSquare = -1
    }
}
