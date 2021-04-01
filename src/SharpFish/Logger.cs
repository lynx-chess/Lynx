using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFish
{
    public static class Logger
    {
        [Conditional("DEBUG")]
        public static void Write(string str)
        {
            Console.Write(str);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        [Conditional("DEBUG")]
        public static void WriteLine()
        {
            Console.WriteLine();
        }
    }
}
