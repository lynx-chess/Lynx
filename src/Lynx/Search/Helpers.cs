namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// Branch-optimized for <paramref name="mostLikely"/>
        /// </summary>
        /// <param name="mostLikely"></param>
        /// <param name="lessLikely"></param>
        /// <returns></returns>
        private static int Max(int mostLikely, int lessLikely)
        {
            return lessLikely <= mostLikely ? mostLikely : lessLikely;
        }

        /// <summary>
        /// Branch-optimized for <paramref name="mostLikely"/>
        /// </summary>
        /// <param name="mostLikely"></param>
        /// <param name="lessLikely"></param>
        /// <returns></returns>
        private static int Min(int mostLikely, int lessLikely)
        {
            return lessLikely >= mostLikely ? mostLikely : lessLikely;
        }
    }
}
