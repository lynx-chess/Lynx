/*
 *
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 10.0.114
 *    [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
 *
 *  Standalone run, with a copy of GoCommand.IsValidUCIMove
 *
 *  | Method                  | Mean      | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------------ |----------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Manual                  |  19.16 ns | 0.364 ns | 0.341 ns |  1.00 |    0.02 |         - |          NA |
 *  | GeneratedRegexIsMatch   | 129.75 ns | 2.545 ns | 2.257 ns |  6.77 |    0.16 |         - |          NA |
 *  | CompiledRegexIsMatch    | 149.94 ns | 2.722 ns | 2.413 ns |  7.83 |    0.18 |         - |          NA |
 *  | InterpretedRegexIsMatch | 346.66 ns | 6.616 ns | 7.353 ns | 18.09 |    0.49 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.UCI.Commands.GUI;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

/// <summary>
/// Hand-written <see cref="GoCommand.IsValidUCIMove(ReadOnlySpan{char})"/> vs regex alternatives
/// </summary>
public partial class IsValidUCIMove_Regex_Benchmark : BaseBenchmark
{
    private const string Pattern = "^[a-h][1-8][a-h][1-8][nbrqNBRQ]?$";

    [GeneratedRegex(Pattern)]
    private static partial Regex GeneratedMoveRegex();

    private static readonly Regex _interpretedRegex = new(Pattern);

    private static readonly Regex _compiledRegex = new(Pattern, RegexOptions.Compiled);

    /// <summary>
    /// Typical mix: valid moves, a promotion, the keywords that end the list and a few rejects
    /// </summary>
    private readonly string[] _tokens = ["e2e4", "d2d4", "g1f3", "a7a8q", "depth", "wtime", "12", "E2E4"];

    [Benchmark(Baseline = true)]
    public int Manual()
    {
        var count = 0;

        foreach (var token in _tokens)
        {
            if (GoCommand.IsValidUCIMove(token))
            {
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int GeneratedRegexIsMatch()
    {
        var count = 0;
        var regex = GeneratedMoveRegex();

        foreach (var token in _tokens)
        {
            if (regex.IsMatch(token.AsSpan()))
            {
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int CompiledRegexIsMatch()
    {
        var count = 0;

        foreach (var token in _tokens)
        {
            if (_compiledRegex.IsMatch(token.AsSpan()))
            {
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int InterpretedRegexIsMatch()
    {
        var count = 0;

        foreach (var token in _tokens)
        {
            if (_interpretedRegex.IsMatch(token.AsSpan()))
            {
                ++count;
            }
        }

        return count;
    }
}
