using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFish.Benchmark
{
    public class Dummy : BaseBenchmark
    {
        [Benchmark]
        public void A()
        {
            AA().ToList();
        }

        [Benchmark]
        public void B()
        {
            BB();
        }

        public static IEnumerable<int> AA()
        {
            for (int i = 0; i < 10_000; ++i)
            {
                if (i % 2 == 0)
                {
                    yield return i;
                }
            }
        }

        public static ICollection<int> BB()
        {
            var candidates = new List<int>(10_000);
            for (int i = 0; i < 10_000; ++i)
            {
                candidates.Add(i);
            }

            return candidates.Where(c => c % 2 == 0).ToList();
        }
    }
}
