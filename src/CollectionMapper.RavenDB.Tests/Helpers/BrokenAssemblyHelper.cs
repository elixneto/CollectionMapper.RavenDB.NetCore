using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace CollectionMapper.RavenDB.Tests.Helpers;

internal static class BrokenAssemblyHelper
{
    public static Assembly Create()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "RavenDBMapperTests_" + Guid.NewGuid().ToString("N"));
        // Diretórios separados: o contexto padrão só sonda o diretório do assembly carregado,
        // portanto não encontrará o fantasma quando tentar resolver PhantomBase.
        var phantomDir = Path.Combine(tempDir, "phantom");
        var brokenDir = Path.Combine(tempDir, "broken");
        Directory.CreateDirectory(phantomDir);
        Directory.CreateDirectory(brokenDir);

        // Passo 1: cria e salva o assembly "fantasma" em phantomDir
        var phantomName = new AssemblyName("PhantomDep_" + Guid.NewGuid().ToString("N"));
        var phantomBuilder = new PersistedAssemblyBuilder(phantomName, typeof(object).Assembly);
        var phantomMod = phantomBuilder.DefineDynamicModule("PhantomModule");
        var phantomType = phantomMod.DefineType("PhantomBase", TypeAttributes.Public | TypeAttributes.Class);
        phantomType.CreateType();
        var phantomPath = Path.Combine(phantomDir, phantomName.Name + ".dll");
        phantomBuilder.Save(phantomPath);

        // Passo 2: carrega o fantasma apenas num AssemblyLoadContext coletável —
        // o contexto padrão nunca o recebe, então não poderá resolvê-lo depois.
        var collectibleCtx = new AssemblyLoadContext("phantom-ctx", isCollectible: true);
        var phantomAssembly = collectibleCtx.LoadFromAssemblyPath(phantomPath);
        var phantomBaseType = phantomAssembly.GetType("PhantomBase")!;

        // Passo 3: cria o assembly "quebrado" (BrokenType : PhantomBase) em brokenDir
        var testName = new AssemblyName("BrokenAssembly_" + Guid.NewGuid().ToString("N"));
        var testBuilder = new PersistedAssemblyBuilder(testName, typeof(object).Assembly);
        var testMod = testBuilder.DefineDynamicModule("TestModule");
        var brokenType = testMod.DefineType("BrokenType", TypeAttributes.Public | TypeAttributes.Class);
        brokenType.SetParent(phantomBaseType);
        brokenType.CreateType();
        var testPath = Path.Combine(brokenDir, testName.Name + ".dll");
        testBuilder.Save(testPath);

        // Passo 4: carrega o assembly "quebrado" no contexto padrão.
        // GetTypes() lançará ReflectionTypeLoadException porque o contexto padrão
        // não encontra PhantomBase: ela está em phantomDir, não em brokenDir.
        return Assembly.LoadFrom(testPath);
    }
}
