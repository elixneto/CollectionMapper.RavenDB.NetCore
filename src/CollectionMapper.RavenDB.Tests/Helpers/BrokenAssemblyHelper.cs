using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace CollectionMapper.RavenDB.Tests.Helpers;

internal static class BrokenAssemblyHelper
{
    public static Assembly Create()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "RavenDBMapperTests_" + Guid.NewGuid().ToString("N"));
        // Separate directories: the default context only probes the loaded assembly's directory,
        // so it will not find the phantom when trying to resolve PhantomBase.
        var phantomDir = Path.Combine(tempDir, "phantom");
        var brokenDir = Path.Combine(tempDir, "broken");
        Directory.CreateDirectory(phantomDir);
        Directory.CreateDirectory(brokenDir);

        // Step 1: create and save the phantom assembly to phantomDir
        var phantomName = new AssemblyName("PhantomDep_" + Guid.NewGuid().ToString("N"));
        var phantomBuilder = new PersistedAssemblyBuilder(phantomName, typeof(object).Assembly);
        var phantomMod = phantomBuilder.DefineDynamicModule("PhantomModule");
        var phantomType = phantomMod.DefineType("PhantomBase", TypeAttributes.Public | TypeAttributes.Class);
        phantomType.CreateType();
        var phantomPath = Path.Combine(phantomDir, phantomName.Name + ".dll");
        phantomBuilder.Save(phantomPath);

        // Step 2: load the phantom only into a collectible AssemblyLoadContext —
        // the default context never receives it, so it cannot resolve it later.
        var collectibleCtx = new AssemblyLoadContext("phantom-ctx", isCollectible: true);
        var phantomAssembly = collectibleCtx.LoadFromAssemblyPath(phantomPath);
        var phantomBaseType = phantomAssembly.GetType("PhantomBase")!;

        // Step 3: create the broken assembly (BrokenType : PhantomBase) in brokenDir
        var testName = new AssemblyName("BrokenAssembly_" + Guid.NewGuid().ToString("N"));
        var testBuilder = new PersistedAssemblyBuilder(testName, typeof(object).Assembly);
        var testMod = testBuilder.DefineDynamicModule("TestModule");
        var brokenType = testMod.DefineType("BrokenType", TypeAttributes.Public | TypeAttributes.Class);
        brokenType.SetParent(phantomBaseType);
        brokenType.CreateType();
        var testPath = Path.Combine(brokenDir, testName.Name + ".dll");
        testBuilder.Save(testPath);

        // Step 4: load the broken assembly into the default context.
        // GetTypes() will throw ReflectionTypeLoadException because the default context
        // cannot find PhantomBase: it lives in phantomDir, not in brokenDir.
        return Assembly.LoadFrom(testPath);
    }
}
