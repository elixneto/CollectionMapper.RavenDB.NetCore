using BenchmarkDotNet.Attributes;
using CollectionMapper.RavenDB;
using CollectionMapper.RavenDB.Benchmarks.Benchmarks.DummyEntities;

namespace CollectionMapper.RavenDB.Benchmarks.Benchmarks;

/// <summary>
/// Measures allocations when chaining Map&lt;T&gt;() calls with compile-time types.
/// Uses 100 static dummy types cycled N times to reach EntryCount calls.
/// At EntryCount &gt; 100, repeated types trigger the O(n) FindIndex path in List&lt;T&gt;.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class MapperFluentChainBenchmark
{
    [Params(100, 1000, 5000, 10000)]
    public int EntryCount { get; set; }

    private RavenDBCollectionMapper _mapper = null!;

    [IterationSetup]
    public void IterationSetup() => _mapper = new RavenDBCollectionMapper();

    [Benchmark]
    public RavenDBCollectionMapper FluentChain()
    {
        var m = _mapper;
        int fullCycles = EntryCount / 100;
        int remainder = EntryCount % 100;

        for (int i = 0; i < fullCycles; i++)
            m = ApplyAll100(m);

        m = ApplyFirst(m, remainder);
        return m;
    }

    private static RavenDBCollectionMapper ApplyAll100(RavenDBCollectionMapper m) =>
        m
         .Map<D001>("Col001")
         .Map<D002>("Col002")
         .Map<D003>("Col003")
         .Map<D004>("Col004")
         .Map<D005>("Col005")
         .Map<D006>("Col006")
         .Map<D007>("Col007")
         .Map<D008>("Col008")
         .Map<D009>("Col009")
         .Map<D010>("Col010")
         .Map<D011>("Col011")
         .Map<D012>("Col012")
         .Map<D013>("Col013")
         .Map<D014>("Col014")
         .Map<D015>("Col015")
         .Map<D016>("Col016")
         .Map<D017>("Col017")
         .Map<D018>("Col018")
         .Map<D019>("Col019")
         .Map<D020>("Col020")
         .Map<D021>("Col021")
         .Map<D022>("Col022")
         .Map<D023>("Col023")
         .Map<D024>("Col024")
         .Map<D025>("Col025")
         .Map<D026>("Col026")
         .Map<D027>("Col027")
         .Map<D028>("Col028")
         .Map<D029>("Col029")
         .Map<D030>("Col030")
         .Map<D031>("Col031")
         .Map<D032>("Col032")
         .Map<D033>("Col033")
         .Map<D034>("Col034")
         .Map<D035>("Col035")
         .Map<D036>("Col036")
         .Map<D037>("Col037")
         .Map<D038>("Col038")
         .Map<D039>("Col039")
         .Map<D040>("Col040")
         .Map<D041>("Col041")
         .Map<D042>("Col042")
         .Map<D043>("Col043")
         .Map<D044>("Col044")
         .Map<D045>("Col045")
         .Map<D046>("Col046")
         .Map<D047>("Col047")
         .Map<D048>("Col048")
         .Map<D049>("Col049")
         .Map<D050>("Col050")
         .Map<D051>("Col051")
         .Map<D052>("Col052")
         .Map<D053>("Col053")
         .Map<D054>("Col054")
         .Map<D055>("Col055")
         .Map<D056>("Col056")
         .Map<D057>("Col057")
         .Map<D058>("Col058")
         .Map<D059>("Col059")
         .Map<D060>("Col060")
         .Map<D061>("Col061")
         .Map<D062>("Col062")
         .Map<D063>("Col063")
         .Map<D064>("Col064")
         .Map<D065>("Col065")
         .Map<D066>("Col066")
         .Map<D067>("Col067")
         .Map<D068>("Col068")
         .Map<D069>("Col069")
         .Map<D070>("Col070")
         .Map<D071>("Col071")
         .Map<D072>("Col072")
         .Map<D073>("Col073")
         .Map<D074>("Col074")
         .Map<D075>("Col075")
         .Map<D076>("Col076")
         .Map<D077>("Col077")
         .Map<D078>("Col078")
         .Map<D079>("Col079")
         .Map<D080>("Col080")
         .Map<D081>("Col081")
         .Map<D082>("Col082")
         .Map<D083>("Col083")
         .Map<D084>("Col084")
         .Map<D085>("Col085")
         .Map<D086>("Col086")
         .Map<D087>("Col087")
         .Map<D088>("Col088")
         .Map<D089>("Col089")
         .Map<D090>("Col090")
         .Map<D091>("Col091")
         .Map<D092>("Col092")
         .Map<D093>("Col093")
         .Map<D094>("Col094")
         .Map<D095>("Col095")
         .Map<D096>("Col096")
         .Map<D097>("Col097")
         .Map<D098>("Col098")
         .Map<D099>("Col099")
         .Map<D100>("Col100");

    private static RavenDBCollectionMapper ApplyFirst(RavenDBCollectionMapper m, int count)
    {
        if (count >= 1) m = m.Map<D001>("Col001");
        if (count >= 2) m = m.Map<D002>("Col002");
        if (count >= 3) m = m.Map<D003>("Col003");
        if (count >= 4) m = m.Map<D004>("Col004");
        if (count >= 5) m = m.Map<D005>("Col005");
        if (count >= 6) m = m.Map<D006>("Col006");
        if (count >= 7) m = m.Map<D007>("Col007");
        if (count >= 8) m = m.Map<D008>("Col008");
        if (count >= 9) m = m.Map<D009>("Col009");
        if (count >= 10) m = m.Map<D010>("Col010");
        if (count >= 11) m = m.Map<D011>("Col011");
        if (count >= 12) m = m.Map<D012>("Col012");
        if (count >= 13) m = m.Map<D013>("Col013");
        if (count >= 14) m = m.Map<D014>("Col014");
        if (count >= 15) m = m.Map<D015>("Col015");
        if (count >= 16) m = m.Map<D016>("Col016");
        if (count >= 17) m = m.Map<D017>("Col017");
        if (count >= 18) m = m.Map<D018>("Col018");
        if (count >= 19) m = m.Map<D019>("Col019");
        if (count >= 20) m = m.Map<D020>("Col020");
        if (count >= 21) m = m.Map<D021>("Col021");
        if (count >= 22) m = m.Map<D022>("Col022");
        if (count >= 23) m = m.Map<D023>("Col023");
        if (count >= 24) m = m.Map<D024>("Col024");
        if (count >= 25) m = m.Map<D025>("Col025");
        if (count >= 26) m = m.Map<D026>("Col026");
        if (count >= 27) m = m.Map<D027>("Col027");
        if (count >= 28) m = m.Map<D028>("Col028");
        if (count >= 29) m = m.Map<D029>("Col029");
        if (count >= 30) m = m.Map<D030>("Col030");
        if (count >= 31) m = m.Map<D031>("Col031");
        if (count >= 32) m = m.Map<D032>("Col032");
        if (count >= 33) m = m.Map<D033>("Col033");
        if (count >= 34) m = m.Map<D034>("Col034");
        if (count >= 35) m = m.Map<D035>("Col035");
        if (count >= 36) m = m.Map<D036>("Col036");
        if (count >= 37) m = m.Map<D037>("Col037");
        if (count >= 38) m = m.Map<D038>("Col038");
        if (count >= 39) m = m.Map<D039>("Col039");
        if (count >= 40) m = m.Map<D040>("Col040");
        if (count >= 41) m = m.Map<D041>("Col041");
        if (count >= 42) m = m.Map<D042>("Col042");
        if (count >= 43) m = m.Map<D043>("Col043");
        if (count >= 44) m = m.Map<D044>("Col044");
        if (count >= 45) m = m.Map<D045>("Col045");
        if (count >= 46) m = m.Map<D046>("Col046");
        if (count >= 47) m = m.Map<D047>("Col047");
        if (count >= 48) m = m.Map<D048>("Col048");
        if (count >= 49) m = m.Map<D049>("Col049");
        if (count >= 50) m = m.Map<D050>("Col050");
        if (count >= 51) m = m.Map<D051>("Col051");
        if (count >= 52) m = m.Map<D052>("Col052");
        if (count >= 53) m = m.Map<D053>("Col053");
        if (count >= 54) m = m.Map<D054>("Col054");
        if (count >= 55) m = m.Map<D055>("Col055");
        if (count >= 56) m = m.Map<D056>("Col056");
        if (count >= 57) m = m.Map<D057>("Col057");
        if (count >= 58) m = m.Map<D058>("Col058");
        if (count >= 59) m = m.Map<D059>("Col059");
        if (count >= 60) m = m.Map<D060>("Col060");
        if (count >= 61) m = m.Map<D061>("Col061");
        if (count >= 62) m = m.Map<D062>("Col062");
        if (count >= 63) m = m.Map<D063>("Col063");
        if (count >= 64) m = m.Map<D064>("Col064");
        if (count >= 65) m = m.Map<D065>("Col065");
        if (count >= 66) m = m.Map<D066>("Col066");
        if (count >= 67) m = m.Map<D067>("Col067");
        if (count >= 68) m = m.Map<D068>("Col068");
        if (count >= 69) m = m.Map<D069>("Col069");
        if (count >= 70) m = m.Map<D070>("Col070");
        if (count >= 71) m = m.Map<D071>("Col071");
        if (count >= 72) m = m.Map<D072>("Col072");
        if (count >= 73) m = m.Map<D073>("Col073");
        if (count >= 74) m = m.Map<D074>("Col074");
        if (count >= 75) m = m.Map<D075>("Col075");
        if (count >= 76) m = m.Map<D076>("Col076");
        if (count >= 77) m = m.Map<D077>("Col077");
        if (count >= 78) m = m.Map<D078>("Col078");
        if (count >= 79) m = m.Map<D079>("Col079");
        if (count >= 80) m = m.Map<D080>("Col080");
        if (count >= 81) m = m.Map<D081>("Col081");
        if (count >= 82) m = m.Map<D082>("Col082");
        if (count >= 83) m = m.Map<D083>("Col083");
        if (count >= 84) m = m.Map<D084>("Col084");
        if (count >= 85) m = m.Map<D085>("Col085");
        if (count >= 86) m = m.Map<D086>("Col086");
        if (count >= 87) m = m.Map<D087>("Col087");
        if (count >= 88) m = m.Map<D088>("Col088");
        if (count >= 89) m = m.Map<D089>("Col089");
        if (count >= 90) m = m.Map<D090>("Col090");
        if (count >= 91) m = m.Map<D091>("Col091");
        if (count >= 92) m = m.Map<D092>("Col092");
        if (count >= 93) m = m.Map<D093>("Col093");
        if (count >= 94) m = m.Map<D094>("Col094");
        if (count >= 95) m = m.Map<D095>("Col095");
        if (count >= 96) m = m.Map<D096>("Col096");
        if (count >= 97) m = m.Map<D097>("Col097");
        if (count >= 98) m = m.Map<D098>("Col098");
        if (count >= 99) m = m.Map<D099>("Col099");
        if (count >= 100) m = m.Map<D100>("Col100");
        return m;
    }
}
