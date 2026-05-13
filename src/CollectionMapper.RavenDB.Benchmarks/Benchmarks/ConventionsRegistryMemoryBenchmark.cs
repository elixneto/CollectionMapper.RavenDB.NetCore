using BenchmarkDotNet.Attributes;
using CollectionMapper.RavenDB;
using CollectionMapper.RavenDB.Benchmarks.Infrastructure;

namespace CollectionMapper.RavenDB.Benchmarks.Benchmarks;

/// <summary>
/// Measures total allocations for registering N types into the static
/// ConcurrentDictionary via RavenDBMapperConventions.RegisterMapper().
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class ConventionsRegistryMemoryBenchmark
{
    [Params(100, 1000, 5000, 10000)]
    public int EntryCount { get; set; }

    private Type[] _types = [];

    [GlobalSetup]
    public void GlobalSetup() => _types = DynamicTypeFactory.GetTypes(EntryCount);

    [IterationSetup]
    public void IterationSetup() => RegistryResetHelper.Reset();

    [IterationCleanup]
    public void IterationCleanup() => RegistryResetHelper.Reset();

    [Benchmark]
    public void RegisterNTypes()
    {
        var mapper = BenchmarkMapperFactory.CreatePopulated(_types);
        RavenDBMapperConventions.RegisterMapper(mapper);
    }
}
