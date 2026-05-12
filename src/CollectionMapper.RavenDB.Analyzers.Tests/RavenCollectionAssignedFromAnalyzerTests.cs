using CollectionMapper.RavenDB.Analyzers.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Xunit;

namespace CollectionMapper.RavenDB.Analyzers.Tests;

public class RavenCollectionAssignedFromAnalyzerTests
{
    // Inline attribute stub so every test compilation is self-contained.
    // The 'using' for the attribute's own namespace is included here so it comes before
    // all namespace/type declarations in the concatenated test source.
    private const string AttributeStub = """
        using System;
        using CollectionMapper.RavenDB.Attributes;
        namespace CollectionMapper.RavenDB.Attributes
        {
            [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            public sealed class RavenCollectionAssignedFromAttribute<T> : Attribute where T : class
            {
                public Type BaseType => typeof(T);
            }
        }
        """;

    // -----------------------------------------------------------------------
    // No-diagnostic cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ClassInheritingFromT_NoDiagnostic()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Apple : Fruit { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ClassInheritingIndirectlyFromT_NoDiagnostic()
    {
        // Grape → Apple → Fruit still satisfies InheritsFrom(Fruit)
        var source = AttributeStub + """
            public class Fruit { }
            public class Apple : Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Grape : Apple { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ClassWithoutAttribute_NoDiagnostic()
    {
        var source = AttributeStub + """
            public class Fruit { }
            public class Apple { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task AttributeNotReferenced_NoDiagnostic()
    {
        // Without the attribute stub the type can't be resolved — analyzer must not crash
        var source = """
            public class Fruit { }
            public class Apple : Fruit { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task AttributeWithNullClass_NoDiagnostic()
    {
        // No stub — SomeUnknownAttribute is unknown to Roslyn → attr.AttributeClass is ErrorType
        var source = """
            [SomeUnknownAttribute]
            public class Apple { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task AttributeUsedWithoutTypeArgument_NoDiagnostic()
    {
        var source = AttributeStub + """
                                     public class Fruit { }
                                     [RavenCollectionAssignedFrom]
                                     public class Apple { }
                                     """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    // -----------------------------------------------------------------------
    // Error cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ClassNotInheritingFromT_ProducesError()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Apple { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        var diag = Assert.Single(diagnostics);
        Assert.Equal(RavenCollectionAssignedFromAnalyzer.DiagnosticId, diag.Id);
        Assert.Equal(DiagnosticSeverity.Error, diag.Severity);
        Assert.Contains("Apple", diag.GetMessage());
        Assert.Contains("Fruit", diag.GetMessage());
    }

    [Fact]
    public async Task ClassInheritingUnrelatedType_ProducesError()
    {
        var source = AttributeStub + """
            public class Fruit { }
            public class Vehicle { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Car : Vehicle { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        var diag = Assert.Single(diagnostics);
        Assert.Equal(RavenCollectionAssignedFromAnalyzer.DiagnosticId, diag.Id);
        Assert.Equal(DiagnosticSeverity.Error, diag.Severity);
        Assert.Contains("'Car' is decorated", diag.GetMessage());
        Assert.Contains("does not inherit from 'Fruit'", diag.GetMessage());
    }

    [Fact]
    public async Task MultipleClassesWithViolations_EachGetsOwnError()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Apple { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Grape { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        Assert.Equal(2, diagnostics.Length);
        Assert.All(diagnostics, d =>
        {
            Assert.Equal(RavenCollectionAssignedFromAnalyzer.DiagnosticId, d.Id);
            Assert.Equal(DiagnosticSeverity.Error, d.Severity);
        });
        Assert.Contains(diagnostics, d => d.GetMessage().Contains("'Apple' is decorated"));
        Assert.Contains(diagnostics, d => d.GetMessage().Contains("'Grape' is decorated"));
    }

    [Fact]
    public async Task MixedValidAndInvalidClasses_OnlyInvalidGetsError()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Apple : Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Grape { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        var diag = Assert.Single(diagnostics);
        Assert.Equal(RavenCollectionAssignedFromAnalyzer.DiagnosticId, diag.Id);
        Assert.Contains("Grape", diag.GetMessage());
        Assert.DoesNotContain("Apple", diag.GetMessage());
    }

    [Fact]
    public async Task DiagnosticMessage_ContainsBothTypeAndBaseTypeName()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Banana { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        var message = Assert.Single(diagnostics).GetMessage();
        Assert.Contains("Banana", message);
        Assert.Contains("Fruit", message);
    }

    [Fact]
    public async Task DiagnosticLocation_PointsToDecoratedClass()
    {
        var source = AttributeStub + """
            public class Fruit { }
            [RavenCollectionAssignedFrom<Fruit>]
            public class Apple { }
            """;

        var diagnostics = await AnalyzerRunner.GetDiagnosticsAsync(source);

        var diag = Assert.Single(diagnostics);
        // Location must be inside the source (not Location.None)
        Assert.True(diag.Location != Location.None);
        Assert.Contains("Apple", diag.Location.SourceTree!.GetText().ToString()
            .Substring(diag.Location.SourceSpan.Start, diag.Location.SourceSpan.Length));
    }
}
