using BenchmarkDotNet.Attributes;
using CollectionMapper.RavenDB;
using CollectionMapper.RavenDB.Benchmarks.Infrastructure;
using CollectionMapper.RavenDB.Models;
using System.Reflection;

namespace CollectionMapper.RavenDB.Benchmarks.Benchmarks;

/// <summary>
/// Measures total allocation cost of creating and populating a RavenDBCollectionMapper
/// with N entries.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class CollectionMapperCreateBenchmark
{
    [Params(100, 1000, 5000, 10000)]
    public int EntryCount { get; set; }

    private Type[] _types = [];

    [GlobalSetup]
    public void GlobalSetup() => _types = DynamicTypeFactory.GetTypes(EntryCount);

    [Benchmark]
    public RavenDBCollectionMapper CreateAndPopulate() =>
        BenchmarkMapperFactory.CreatePopulated(_types);
}

/// <summary>
/// Measures allocation cost of a single append to an already-populated mapper.
/// Exposes the O(n) FindIndex cost of the internal List as EntryCount grows.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class CollectionMapperSingleAddBenchmark
{
    private static readonly FieldInfo MappedCollectionsField =
        typeof(RavenDBCollectionMapper)
            .GetField("_mappedCollections", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [Params(100, 1000, 5000, 10000)]
    public int EntryCount { get; set; }

    private Type[] _types = [];
    private Type _extraType = null!;
    private RavenDBCollectionMapper _mapper = null!;
    private List<RavenDBCollectionDefinitionModel> _list = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _types = DynamicTypeFactory.GetTypes(EntryCount);
        // One extra type beyond EntryCount for the append operation
        _extraType = DynamicTypeFactory.GetTypes(EntryCount + 1)[^1];
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _mapper = BenchmarkMapperFactory.CreatePopulated(_types);
        _list = (List<RavenDBCollectionDefinitionModel>)MappedCollectionsField.GetValue(_mapper)!;
    }

    [Benchmark]
    public void SingleMapAdd() =>
        _list.Add(new RavenDBCollectionDefinitionModel(_extraType, "ExtraCollection"));
}
