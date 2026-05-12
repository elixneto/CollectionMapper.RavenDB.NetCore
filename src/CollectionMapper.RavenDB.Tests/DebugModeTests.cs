using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class DebugModeTests
{
    private static string CaptureConsole(Action action)
    {
        var original = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);
        try
        {
            action();
            return sw.ToString();
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public void RegisterDecorators_WithDebugMode_LogsRegistrations()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.EnableDebugMode();
        var expectedFruits = "[CollectionMapper.RavenDB] ::Register:: CollectionMapper.RavenDB.Tests.Fixtures.Fruit → \"Fruits\" | source: [RavenCollection]";
        var expectedApple = "[CollectionMapper.RavenDB] ::Register:: CollectionMapper.RavenDB.Tests.Fixtures.Apple → \"Fruits\" | source: [RavenCollectionAssignedFrom<Fruit>]";
        var expectedGrape = "[CollectionMapper.RavenDB] ::Register:: CollectionMapper.RavenDB.Tests.Fixtures.Grape → \"Fruits\" | source: [RavenCollectionAssignedFrom<Fruit>]";
        var expectedCar = "[CollectionMapper.RavenDB] ::Register:: CollectionMapper.RavenDB.Tests.Fixtures.Car → \"VehicleBases\" | source: [RavenCollectionAssignedFrom<VehicleBase>]";

        var output = CaptureConsole(() => RavenDBMapperConventions.RegisterByAssembly<Fruit>());

        Assert.Contains(expectedFruits, output);
        Assert.Contains(expectedApple, output);
        Assert.Contains(expectedGrape, output);
        Assert.Contains(expectedCar, output);
    }

    [Fact]
    public void RegisterDecorators_WithDebugMode_LogsBaseTypeResolution()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.EnableDebugMode();

        var output = CaptureConsole(() => RavenDBMapperConventions.RegisterByAssembly<Fruit>());

        // Apple and Grape should appear as registered via AssignedFrom
        Assert.Contains(nameof(Apple), output);
        Assert.Contains(nameof(Grape), output);
    }

    [Fact]
    public void FindCollection_WithDebugMode_LogsRegistryHit()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();
        RavenDBMapperConventions.EnableDebugMode();

        var output = CaptureConsole(() => RavenDBMapperConventions.FindCollection(typeof(Fruit)));

        Assert.StartsWith("[CollectionMapper.RavenDB] ::FindCollection:: CollectionMapper.RavenDB.Tests.Fixtures.Fruit → \"Fruits\"", output);
    }

    [Fact]
    public void FindCollection_WithDebugMode_LogsDefaultFallback()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.EnableDebugMode();

        var output = CaptureConsole(() => RavenDBMapperConventions.FindCollection(typeof(UnmappedDocument)));

        Assert.StartsWith("[CollectionMapper.RavenDB] ::FindCollection:: CollectionMapper.RavenDB.Tests.Fixtures.UnmappedDocument → \"UnmappedDocuments\" (fallback)", output);
    }

    [Fact]
    public void RegisterMapper_WithDebugMode_LogsRegistrations()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.EnableDebugMode();

        var mapper = new RavenDBCollectionMapper();
        mapper.Map<Banana>("Fruits");

        var output = CaptureConsole(() => RavenDBMapperConventions.RegisterMapper(mapper));

        Assert.StartsWith("[CollectionMapper.RavenDB] ::Register:: CollectionMapper.RavenDB.Tests.Fixtures.Banana → \"Fruits\" | source: RavenDBCollectionMapper", output);
    }

    [Fact]
    public void FindCollection_WithoutDebugMode_ProducesNoOutput()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();
        // Debug mode NOT enabled

        var output = CaptureConsole(() => RavenDBMapperConventions.FindCollection(typeof(Fruit)));

        Assert.Empty(output);
    }

    [Fact]
    public void RegisterDecorators_WithoutDebugMode_ProducesNoOutput()
    {
        RegistryResetHelper.Reset();
        // Debug mode NOT enabled

        var output = CaptureConsole(RavenDBMapperConventions.RegisterByAssembly<Fruit>);

        Assert.Empty(output);
    }

    [Fact]
    public void RegisterMapper_Override_WithDebugMode_LogsOverriddenMessage()
    {
        RegistryResetHelper.Reset();
        var m1 = new RavenDBCollectionMapper();
        m1.Map<Banana>("OldName");
        RavenDBMapperConventions.RegisterMapper(m1);
        RavenDBMapperConventions.EnableDebugMode();

        var m2 = new RavenDBCollectionMapper();
        m2.Map<Banana>("NewName");
        var output = CaptureConsole(() => RavenDBMapperConventions.RegisterMapper(m2));

        Assert.Contains("(OVERRIDEN)", output);
        Assert.Contains("NewName", output);
    }
}
