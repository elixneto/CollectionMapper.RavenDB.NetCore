using BenchmarkDotNet.Attributes;
using CollectionMapper.RavenDB;
using CollectionMapper.RavenDB.Benchmarks.Infrastructure;

namespace CollectionMapper.RavenDB.Benchmarks.Benchmarks;

/// <summary>
/// Measures allocations and time of FindCollection(Type) against a registry
/// pre-populated with N entries. Tests both hit (type registered) and miss
/// (falls back to RavenDB's DefaultGetCollectionName) paths.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class FindCollectionLookupBenchmark
{
    [Params(100, 1000, 5000, 10000)]
    public int EntryCount { get; set; }

    private Type _hitType = null!;
    private Type _missType = typeof(object); // never registered

    [GlobalSetup]
    public void GlobalSetup()
    {
        RegistryResetHelper.Reset();

        var types = DynamicTypeFactory.GetTypes(EntryCount);
        var mapper = BenchmarkMapperFactory.CreatePopulated(types);
        RavenDBMapperConventions.RegisterMapper(mapper);

        _hitType = types[EntryCount / 2];
    }

    [GlobalCleanup]
    public void GlobalCleanup() => RegistryResetHelper.Reset();

    /// <summary>
    /// ConcurrentDictionary.TryGetValue — expected O(1), zero allocations.
    /// </summary>
    [Benchmark]
    public string FindHit() => RavenDBMapperConventions.FindCollection(_hitType);

    /// <summary>
    /// TryGetValue misses, delegates to DocumentConventions.DefaultGetCollectionName.
    /// Reveals real fallback cost paid for unregistered types.
    /// </summary>
    [Benchmark]
    public string FindMiss() => RavenDBMapperConventions.FindCollection(_missType);
}
