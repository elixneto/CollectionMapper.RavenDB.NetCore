using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CollectionMapper.RavenDB.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RavenCollectionAssignedFromAnalyzer : DiagnosticAnalyzer
{
    private const string AttributeNamespace = "CollectionMapper.RavenDB.Attributes";
    private const string AttributeMetadataName = "RavenCollectionAssignedFromAttribute`1";
    
    public const string DiagnosticId = "CMRAVEN001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Type does not inherit from the base type specified in RavenCollectionAssignedFrom",
        messageFormat: "'{0}' is decorated with [RavenCollectionAssignedFrom<{1}>] but does not inherit from '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A class decorated with [RavenCollectionAssignedFrom<T>] must inherit from T. Without this inheritance the collection mapping will not work as intended.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        foreach (var attr in typeSymbol.GetAttributes())
        {
            if (attr.AttributeClass is null)
            {
                continue;
            }
            
            if (!IsAssignedFromAttribute(attr.AttributeClass))
            {
                continue;
            }

            if (attr.AttributeClass.TypeArguments.Length != 1)
            {
                continue;
            }

            var requiredBaseType = attr.AttributeClass.TypeArguments[0];

            if (!InheritsFrom(typeSymbol, requiredBaseType))
            {
                var location = typeSymbol.Locations.IsEmpty
                    ? Location.None
                    : typeSymbol.Locations[0];

                context.ReportDiagnostic(Diagnostic.Create(
                    Rule,
                    location,
                    typeSymbol.Name,
                    requiredBaseType.Name));
            }

            // AllowMultiple = false — at most one instance of this attribute per class
            // don't need to check for other attributes of the same type
            break;
        }
    }

    private static bool IsAssignedFromAttribute(INamedTypeSymbol symbol)
    {
        var original = symbol.OriginalDefinition;
        return original.IsGenericType
               && original.MetadataName == AttributeMetadataName
               && original.ContainingNamespace.ToDisplayString() == AttributeNamespace;
    }

    private static bool InheritsFrom(INamedTypeSymbol typeSymbol, ITypeSymbol baseType)
    {
        var current = typeSymbol.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }
}
