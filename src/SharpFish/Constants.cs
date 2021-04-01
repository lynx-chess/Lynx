namespace SharpFish
{
    public static class Constants
    {
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
