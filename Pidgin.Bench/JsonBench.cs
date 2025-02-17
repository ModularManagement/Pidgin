using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using Pidgin.Bench.SpracheParsers;
using Pidgin.Bench.SuperpowerParsers;
using Pidgin.Examples.Json;

namespace Pidgin.Bench;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class JsonBench
{
#nullable disable
    private string _bigJson;
    private string _longJson;
    private string _wideJson;
    private string _deepJson;
#nullable restore

    [GlobalSetup]
    public void Setup()
    {
        _bigJson = BuildJson(4, 4, 3).ToString()!;
        _longJson = BuildJson(256, 1, 1).ToString()!;
        _wideJson = BuildJson(1, 1, 256).ToString()!;
        _deepJson = BuildJson(1, 256, 1).ToString()!;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Big")]
    public void BigJson_Pidgin()
    {
        JsonParser.Parse(_bigJson);
    }

    [Benchmark]
    [BenchmarkCategory("Big")]
    public void BigJson_Sprache()
    {
        SpracheJsonParser.Parse(_bigJson);
    }

    [Benchmark]
    [BenchmarkCategory("Big")]
    public void BigJson_Superpower()
    {
        SuperpowerJsonParser.Parse(_bigJson);
    }

    [Benchmark]
    [BenchmarkCategory("Big")]
    public void BigJson_FParsec()
    {
        Pidgin.Bench.FParsec.JsonParser.parse(_bigJson);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Long")]
    public void LongJson_Pidgin()
    {
        JsonParser.Parse(_longJson);
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public void LongJson_Sprache()
    {
        SpracheJsonParser.Parse(_longJson);
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public void LongJson_Superpower()
    {
        SuperpowerJsonParser.Parse(_longJson);
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public void LongJson_FParsec()
    {
        Pidgin.Bench.FParsec.JsonParser.parse(_longJson);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Deep")]
    public void DeepJson_Pidgin()
    {
        JsonParser.Parse(_deepJson);
    }

    [Benchmark]
    [BenchmarkCategory("Deep")]
    public void DeepJson_Sprache()
    {
        SpracheJsonParser.Parse(_deepJson);
    }

    // this one blows the stack
    // [Benchmark, BenchmarkCategory("Deep")]
    // public void DeepJson_Superpower()
    // {
    //     SuperpowerJsonParser.Parse(_deepJson);
    // }
    [Benchmark]
    [BenchmarkCategory("Deep")]
    public void DeepJson_FParsec()
    {
        Pidgin.Bench.FParsec.JsonParser.parse(_deepJson);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Wide")]
    public void WideJson_Pidgin()
    {
        JsonParser.Parse(_wideJson);
    }

    [Benchmark]
    [BenchmarkCategory("Wide")]
    public void WideJson_Sprache()
    {
        SpracheJsonParser.Parse(_wideJson);
    }

    [Benchmark]
    [BenchmarkCategory("Wide")]
    public void WideJson_Superpower()
    {
        SuperpowerJsonParser.Parse(_wideJson);
    }

    [Benchmark]
    [BenchmarkCategory("Wide")]
    public void WideJson_FParsec()
    {
        Pidgin.Bench.FParsec.JsonParser.parse(_wideJson);
    }

    private static Json BuildJson(int length, int depth, int width)
        => new JsonArray(
            Enumerable.Repeat(1, length)
                .Select(_ => BuildObject(depth, width))
                .ToImmutableArray()
        );

    private static Json BuildObject(int depth, int width)
    {
        if (depth == 0)
        {
            return new JsonString(RandomString(6));
        }

        return new JsonObject(
            Enumerable.Repeat(1, width)
                .Select(_ => new KeyValuePair<string, Json>(RandomString(5), BuildObject(depth - 1, width)))
                .ToImmutableDictionary()
        );
    }

    private static readonly Random _random = new();

    [SuppressMessage("security", "CA5394:Use cryptographically secure random number generators", Justification = "Test code")]
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(
            Enumerable
                .Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
        );
    }
}
