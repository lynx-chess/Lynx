using Lynx.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Result
        {
            public List<Move> Moves { get; set; } = new List<Move>(150);
        }

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

        [Conditional("DEBUG")]
        private static void PrintPreMove(Position position, int depthLeft, Move move)
        {
            var sb = new StringBuilder();
            for (int i = depthLeft; i <= Configuration.Parameters.Depth; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            if (depthLeft > 1)
            {
                //Console.WriteLine($"{Environment.NewLine}{depthStr}{move} ({position.Side}, {depthLeft})");
                Logger.Trace($"{Environment.NewLine}{depthStr}{move} ({position.Side}, {depthLeft})");
            }
        }

        [Conditional("DEBUG")]
        private static void PrintMove(int depthLeft, Move move, int evaluation, Position position)
        {
            var sb = new StringBuilder();
            for (int i = depthLeft; i <= Configuration.Parameters.Depth; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            Console.ForegroundColor = depthLeft switch
            {
                4 => ConsoleColor.Red,
                3 => ConsoleColor.Blue,
                2 => ConsoleColor.Green,
                1 => ConsoleColor.White,
                _ => ConsoleColor.White
            };

            //Console.WriteLine($"{depthStr}{move} ({position.Side}, {depthLeft}) | {evaluation}");
            //Console.WriteLine($"{depthStr}{move} | {evaluation}");
            Logger.Trace($"{depthStr}{move} | {evaluation}");

            Console.ResetColor();
        }
    }
}
