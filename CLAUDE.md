# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from the `src/` directory (where the `.sln` lives).

```bash
# Build entire solution
dotnet build

# Run all library + attribute tests
dotnet test CollectionMapper.RavenDB.Tests/

# Run all analyzer tests
dotnet test CollectionMapper.RavenDB.Analyzers.Tests/

# Run a single test by name
dotnet test CollectionMapper.RavenDB.Tests/ --filter "FullyQualifiedName~FindCollectionFallback"

# Run benchmarks (must be Release)
dotnet run -c Release --project CollectionMapper.RavenDB.Benchmarks -- --filter *

# Pack the NuGet package (build analyzer first)
dotnet build CollectionMapper.RavenDB.Analyzers/ -c Release
dotnet pack CollectionMapper.RavenDB/ -c Release
```

`TreatWarningsAsErrors` is enabled in the main library — keep the build warning-free.

## Architecture

The solution has four projects:

| Project | Role |
|---|---|
| `CollectionMapper.RavenDB` | Main library — multi-targeted (net8/9/10) |
| `CollectionMapper.RavenDB.Analyzers` | Roslyn analyzer — netstandard2.0 |
| `CollectionMapper.RavenDB.Tests` | xUnit tests for the library — net10.0 |
| `CollectionMapper.RavenDB.Analyzers.Tests` | xUnit tests for the analyzer — net10.0 |
| `CollectionMapper.RavenDB.Benchmarks` | BenchmarkDotNet memory benchmarks — net10.0 |

### Core library (`CollectionMapper.RavenDB`)

**`RavenDBMapperConventions`** (static class) — the single entry point consumers wire to `DocumentStore.Conventions.FindCollectionName`. Holds a process-wide `ConcurrentDictionary<Type, string>` (`_collectionsRegistry`). Three registration paths all converge on the private `Register()` method, which applies last-write-wins semantics.

**`RavenDBCollectionMapper`** (instance class) — fluent builder. Internally uses `List<RavenDBCollectionDefinitionModel>` with O(n) `FindIndex` to detect duplicate types before adding. Subclassing this and calling `Map<T>` in the constructor is the auto-discovery pattern — `RegisterByAssembly` instantiates all concrete non-generic subclasses it finds.

**Assembly scanning order** (important — later steps can override earlier ones):
1. Concrete `RavenDBCollectionMapper` subclasses (instantiated and iterated)
2. `[RavenCollection]` attributes — must run before step 3 so base types are in the registry
3. `[RavenCollectionAssignedFrom<T>]` attributes — resolves the base type's collection from the registry, then the attribute, then RavenDB's default

**`FindCollection(Type)`** — hit path is a `ConcurrentDictionary.TryGetValue` (zero allocations); miss path delegates to `DocumentConventions.DefaultGetCollectionName`.

### Roslyn Analyzer (`CollectionMapper.RavenDB.Analyzers`)

A single analyzer: **`RavenCollectionAssignedFromAnalyzer`** (diagnostic `CMRAVEN001`). It fires at compile time when a class is decorated with `[RavenCollectionAssignedFrom<T>]` but does not inherit from `T`. The analyzer is packaged inside the NuGet package and runs automatically for all consumers; it is also wired as an `<Analyzer>` reference in the test project.

The analyzer DLL is included in the NuGet package via a custom `IncludeAnalyzerInPackage` target in the library `.csproj` — the analyzer project itself is not a NuGet dependency, only the compiled output is packed.

### Testing patterns

- **Static state isolation**: `_collectionsRegistry` persists for the process lifetime, so every test class that touches `RavenDBMapperConventions` must use `RegistryResetHelper.Reset()`. Tests that share this state are annotated `[Collection("Serial")]` (see `SerialCollectionDefinition.cs`) to disable xUnit parallelism.
- **Benchmark infrastructure**: mirrors the same reflection-based reset pattern (`Infrastructure/RegistryResetHelper.cs`) and uses `AssemblyBuilder.DefineDynamicAssembly` to generate runtime types, bypassing the `Map<T>` generic constraint.
- **Analyzer tests** compile C# source strings directly via `Microsoft.CodeAnalysis.CSharp` — no framework wrapper, just raw Roslyn.
