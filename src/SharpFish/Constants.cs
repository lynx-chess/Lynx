namespace SharpFish
{
    public static class Constants
    {
        public static readonly string[] Coordinates = new string[]
        {
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
        };

        /// <summary>
        /// 0xFFFFFFFFFFFFFFFFUL
        /// 8   1 1 1 1 1 1 1 1
        /// 7   1 1 1 1 1 1 1 1
        /// 6   1 1 1 1 1 1 1 1
        /// 5   1 1 1 1 1 1 1 1
        /// 4   1 1 1 1 1 1 1 1
        /// 3   1 1 1 1 1 1 1 1
        /// 2   1 1 1 1 1 1 1 1
        /// 1   1 1 1 1 1 1 1 1
        ///     a b c d e f g h
        /// </summary>
        public const ulong FullBoard = ulong.MaxValue;

        /// <summary>
        /// 8   0 1 1 1 1 1 1 1
        /// 7   0 1 1 1 1 1 1 1
        /// 6   0 1 1 1 1 1 1 1
        /// 5   0 1 1 1 1 1 1 1
        /// 4   0 1 1 1 1 1 1 1
        /// 3   0 1 1 1 1 1 1 1
        /// 2   0 1 1 1 1 1 1 1
        /// 1   0 1 1 1 1 1 1 1
        ///     a b c d e f g h
        /// </summary>
        public const ulong NotAFile = 0xFEFEFEFEFEFEFEFE;

        /// <summary>
        /// 8   1 1 1 1 1 1 1 0
        /// 7   1 1 1 1 1 1 1 0
        /// 6   1 1 1 1 1 1 1 0
        /// 5   1 1 1 1 1 1 1 0
        /// 4   1 1 1 1 1 1 1 0
        /// 3   1 1 1 1 1 1 1 0
        /// 2   1 1 1 1 1 1 1 0
        /// 1   1 1 1 1 1 1 1 0
        ///     a b c d e f g h
        /// </summary>
        public const ulong NotHFile = 0x7F7F7F7F7F7F7F7F;

        /// <summary>
        /// 8   1 1 1 1 1 1 0 0
        /// 7   1 1 1 1 1 1 0 0
        /// 6   1 1 1 1 1 1 0 0
        /// 5   1 1 1 1 1 1 0 0
        /// 4   1 1 1 1 1 1 0 0
        /// 3   1 1 1 1 1 1 0 0
        /// 2   1 1 1 1 1 1 0 0
        /// 1   1 1 1 1 1 1 0 0
        ///     a b c d e f g h
        /// </summary>
        public const ulong NotHGFiles = 0x3F3F3F3F3F3F3F3F;

        /// <summary>
        /// 8   0 0 1 1 1 1 1 1
        /// 7   0 0 1 1 1 1 1 1
        /// 6   0 0 1 1 1 1 1 1
        /// 5   0 0 1 1 1 1 1 1
        /// 4   0 0 1 1 1 1 1 1
        /// 3   0 0 1 1 1 1 1 1
        /// 2   0 0 1 1 1 1 1 1
        /// 1   0 0 1 1 1 1 1 1
        ///     a b c d e f g h
        /// </summary>
        public const ulong NotABFiles = 0xFCFCFCFCFCFCFCFC;
    }
}
