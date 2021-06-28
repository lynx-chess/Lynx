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

        private const int MinValue = -2 * Position.CheckMateEvaluation;
        private const int MaxValue = +2 * Position.CheckMateEvaluation;

        public class Result
        {
            public int? MaxDepth { get; set; }

            public List<Move> Moves { get; set; } = new List<Move>(150);
        }

        public record SearchResult(Move BestMove, double Evaluation, int TargetDepth, int DepthReached,  List<Move> Moves);

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
        private static void PrintPreMove(Position position, int plies, Move move, bool isQuiescence = false)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            //if (plies < Configuration.Parameters.Depth - 1)
            {
                //Console.WriteLine($"{Environment.NewLine}{depthStr}{move} ({position.Side}, {depth})");
                Logger.Trace($"{Environment.NewLine}{depthStr}{(isQuiescence ? "[Qui] " : "")}{move} ({position.Side}, {plies})");
            }
        }

        [Conditional("DEBUG")]
        private static void PrintMove(int plies, Move move, int evaluation, Position position, bool isQuiescence = false, bool prune = false)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            //Console.ForegroundColor = depth switch
            //{
            //    0 => ConsoleColor.Red,
            //    1 => ConsoleColor.Blue,
            //    2 => ConsoleColor.Green,
            //    3 => ConsoleColor.White,
            //    _ => ConsoleColor.White
            //};
            //Console.WriteLine($"{depthStr}{move} ({position.Side}, {depthLeft}) | {evaluation}");
            //Console.WriteLine($"{depthStr}{move} | {evaluation}");

            Logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move,-6} | {evaluation}{(prune ? " | prunning" : "")}");

            //Console.ResetColor();
        }

        [Conditional("DEBUG")]
        private static void PrintMessage(int plies, string message)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            Logger.Trace(depthStr + message);
        }
    }
}
