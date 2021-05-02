using System;
using System.Diagnostics;

namespace Lynx.Internal
{
    internal static class Logger
    {
        public static void Write(string str)
        {
            Console.Write(str);
        }

        public static void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        [Conditional("DEBUG")]
        public static void Debug(string str)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[DEBUG]\t{str}");
            Console.ResetColor();
        }

        public static void Warn(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN]\t{str}");
            Console.ResetColor();
        }

        public static void Error(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR]\t{str}");
        }
    }
}
