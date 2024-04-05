using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Generators;

public static class CommonSteps
{
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext>
        ForAttributeWithMetadataName(
            this SyntaxValueProvider source,
            string fullyQualifiedMetadataName)
    {
        return source
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: fullyQualifiedMetadataName,
                predicate: static (node, _) =>
                    node is InterfaceDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (context, _) => context);
    }

    public static IncrementalValuesProvider<(SemanticModel SemanticModel, AttributeData AttributeData, InterfaceDeclarationSyntax InterfaceSyntax, INamedTypeSymbol InterfaceSymbol)>
        SelectManyAllAttributesOfCurrentInterfaceSyntax(
            this IncrementalValuesProvider<GeneratorAttributeSyntaxContext> source)
    {
        return source
            .SelectMany(static (context, _) => context.Attributes
                .Select(x => (
                    context.SemanticModel,
                    AttributeData: x,
                    ClassSyntax: (InterfaceDeclarationSyntax)context.TargetNode,
                    ClassSymbol: (INamedTypeSymbol)context.TargetSymbol)));
    }
}
