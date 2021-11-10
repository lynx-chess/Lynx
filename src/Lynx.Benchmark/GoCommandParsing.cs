/*
 *
 *  |     Method |              command |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |----------- |--------------------- |----------:|---------:|---------:|------:|--------:|-------:|------:|------:|----------:|
 *  | Sequential |          go infinite |  38.06 us | 0.671 us | 0.627 us |  1.00 |    0.00 | 0.1221 |     - |     - |     365 B |
 *  |  Parallell |          go infinite |  16.24 us | 0.318 us | 0.504 us |  0.43 |    0.02 | 1.1597 |     - |     - |    2439 B |
 *  |            |                      |           |          |          |       |         |        |       |       |           |
 *  | Sequential | go in(...) d2d4 [33] |  69.36 us | 0.942 us | 0.881 us |  1.00 |    0.00 | 0.3662 |     - |     - |     825 B |
 *  |  Parallell | go in(...) d2d4 [33] |  27.98 us | 1.005 us | 2.963 us |  0.40 |    0.05 | 1.3733 |     - |     - |    2888 B |
 *  |            |                      |           |          |          |       |         |        |       |       |           |
 *  | Sequential |  go i(...) 500 [137] | 127.31 us | 2.051 us | 2.442 us |  1.00 |    0.00 | 1.2207 |     - |     - |    3040 B |
 *  |  Parallell |  go i(...) 500 [137] |  59.41 us | 2.109 us | 6.219 us |  0.48 |    0.05 | 2.4414 |     - |     - |    5096 B |
 *  |            |                      |           |          |          |       |         |        |       |       |           |
 *  | Sequential | go in(...)c 100 [66] |  90.49 us | 0.943 us | 0.882 us |  1.00 |    0.00 | 0.7324 |     - |     - |    1576 B |
 *  |  Parallell | go in(...)c 100 [66] |  38.29 us | 1.494 us | 4.406 us |  0.42 |    0.05 | 1.7090 |     - |     - |    3632 B |
 *  |            |                      |           |          |          |       |         |        |       |       |           |
 *  | Sequential |  go i(...) 500 [117] | 109.52 us | 1.283 us | 1.002 us |  1.00 |    0.00 | 1.2207 |     - |     - |    2776 B |
 *  |  Parallell |  go i(...) 500 [117] |  51.02 us | 1.942 us | 5.727 us |  0.48 |    0.05 | 2.3193 |     - |     - |    4832 B |
 *
 */

using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark
{
    public class GoCommandParsing : BaseBenchmark
    {
        public static IEnumerable<string> Data => new[] {
            "go infinite",
            "go infinite searchmoves e2e4 d2d4",
            "go infinite searchmoves e2e4 d2d4 wtime 10000 btime 10000 winc 100",
            "go infinite searchmoves e2e4 d2d4 wtime 10000 btime 10000 winc 100 binc 100 movestogo 10 depth50 mate 10 movetime 500",
            "go infinite searchmoves e2e4 d2d4 a2a4 a2a3 b2b4 b2b3 wtime 10000 btime 10000 winc 100 binc 100 movestogo 10 depth50 mate 10 movetime 500"
        };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public async Task Sequential(string command)
        {
            await Task.Run(() => ParseSequentially(command));
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public async Task Parallell(string command)
        {
            await ParseInParallel(command);
        }

        private readonly Regex _searchMovesRegex = new(
            "(?<=searchmoves).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _whiteTimeRegex = new(
            "(?<=wtime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _blackTimeRegex = new(
            "(?<=btime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _whiteIncrementRegex = new(
            "(?<=winc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _blackIncrementRegex = new(
            "(?<=binc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _movesToGoRegex = new(
            "(?<=movestogo).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _depthRegex = new(
            "(?<=depth).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _nodesRegex = new(
            "(?<=nodes).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _mateRegex = new(
            "(?<=mate).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _moveTimeRegex = new(
            "(?<=movetime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public List<string> SearchMoves { get; private set; } = default!;
        public int WhiteTime { get; private set; } = default!;
        public int BlackTime { get; private set; } = default!;
        public int WhiteIncrement { get; private set; } = default!;
        public int BlackIncrement { get; private set; } = default!;
        public int MovesToGo { get; private set; } = default!;
        public int Depth { get; private set; } = default!;
        public int Nodes { get; private set; } = default!;
        public int Mate { get; private set; } = default!;
        public int MoveTime { get; private set; } = default!;
        public bool Infinite { get; private set; } = default!;
        public bool Ponder { get; private set; } = default!;

        private void ParseSequentially(string command)
        {
            var match = _searchMovesRegex.Match(command);
            SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            match = _whiteTimeRegex.Match(command);
            if (int.TryParse(match.Value, out var value))
            {
                WhiteTime = value;
            }

            match = _blackTimeRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                BlackTime = value;
            }

            match = _whiteIncrementRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                WhiteIncrement = value;
            }

            match = _blackIncrementRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                BlackIncrement = value;
            }

            match = _movesToGoRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                MovesToGo = value;
            }

            match = _depthRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                Depth = value;
            }

            match = _nodesRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                Nodes = value;
            }

            match = _mateRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                Mate = value;
            }

            match = _moveTimeRegex.Match(command);
            if (int.TryParse(match.Value, out value))
            {
                MoveTime = value;
            }

            Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase);

            Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ParseInParallel(string command)
        {
            var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = _searchMovesRegex.Match(command);

                    SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }),
                Task.Run(() =>
                {
                    var match = _whiteTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _whiteIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _movesToGoRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _depthRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _nodesRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _mateRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _moveTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
                    }
                }),
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase))
            };

            await Task.WhenAll(taskList);
        }
    }
}
