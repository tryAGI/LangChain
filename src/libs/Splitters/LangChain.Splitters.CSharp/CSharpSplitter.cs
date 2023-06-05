using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace LangChain.Splitters;

/// <summary>
/// 
/// </summary>
public class CSharpSplitter : ISplitter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<DocumentPart>> SplitAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        var components = new List<DocumentPart>();

        var tree = CSharpSyntaxTree.ParseText(content, cancellationToken: cancellationToken);
        var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

        foreach (var constructor in root.DescendantNodes().OfType<ConstructorDeclarationSyntax>())
        {
            components.Add(new DocumentPart(
                Name: constructor.Identifier.ToFullString(),
                Content: constructor.ToFullString(),
                Type: DocumentPartType.Constructor));
        }
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            components.Add(new DocumentPart(
                Name: method.Identifier.ToFullString(),
                Content: method.ToFullString(),
                Type: DocumentPartType.Method));
        }

        return components
            .GroupBy(static x => x.Name)
            .Select(static x => new DocumentPart(
                Name: x.Key,
                Content: string.Join(Environment.NewLine, x),
                Type: x.First().Type))
            .ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="requiredNames"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> CutAsync(
        string content,
        IReadOnlyCollection<string> requiredNames,
        CancellationToken cancellationToken = default)
    {
        requiredNames = requiredNames ?? throw new ArgumentNullException(nameof(requiredNames));
        
        var tree = CSharpSyntaxTree.ParseText(content, cancellationToken: cancellationToken);
        var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

        // Create a new compilation unit and class
        var newCompilationUnit = SyntaxFactory.CompilationUnit();
        var oldClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        var newClass = SyntaxFactory.ClassDeclaration(oldClass.Identifier);

        // Add fields and properties
        foreach (var field in oldClass.Members.OfType<FieldDeclarationSyntax>())
        {
            newClass = newClass.AddMembers(field);
        }
        foreach (var property in oldClass.Members.OfType<PropertyDeclarationSyntax>())
        {
            newClass = newClass.AddMembers(property);
        }

        // Add specified constructors and methods
        var oldConstructors = root
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .Select(static x => (Name: x.Identifier.ToFullString(), x))
            .ToArray();
        var oldMethods = root
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Select(static x => (Name: x.Identifier.ToFullString(), x))
            .ToArray();
        foreach (var member in requiredNames)
        {
            foreach (var (_, constructor) in oldConstructors
                         .Where(tuple => tuple.Name == member))
            {
                newClass = newClass.AddMembers(constructor);
            }
            foreach (var (_, method) in oldMethods
                         .Where(tuple => tuple.Name == member))
            {
                newClass = newClass.AddMembers(method);
            }
        }

        // Add new class to the compilation unit
        newCompilationUnit = newCompilationUnit.AddMembers(newClass);

        // Format the new code
        using var workspace = new AdhocWorkspace();
        var formattedNode = Formatter.Format(newCompilationUnit, workspace, cancellationToken: cancellationToken);

        return formattedNode.ToFullString();
    }
}