var solutionDirectory = args.ElementAtOrDefault(0) ?? Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../.."));
var sampleDirectory = Path.Combine(solutionDirectory, "examples");
var mkDocsPath = Path.Combine(solutionDirectory, "mkdocs.yml");

Console.WriteLine($"Generating samples from {sampleDirectory}...");
foreach (var path in Directory.EnumerateFiles(sampleDirectory, "Program.cs", SearchOption.AllDirectories))
{
    var folder = Path.GetFileName(Path.GetDirectoryName(path) ?? string.Empty)?.Replace("LangChain.Samples.", string.Empty);
    var code = await File.ReadAllTextAsync(path);

    var newDir = Path.Combine(solutionDirectory, "docs", "samples");
    Directory.CreateDirectory(newDir);

    var newPath = Path.Combine(newDir, $"{folder}.md");
    await File.WriteAllTextAsync(newPath, $@"```csharp
{code}
```");
}

var metaTestsFolder = Path.Combine(solutionDirectory, "src", "Meta", "test");
Console.WriteLine($"Generating samples from {metaTestsFolder}...");
foreach (var path in Directory.EnumerateFiles(metaTestsFolder, "WikiTests.*.cs", SearchOption.AllDirectories))
{
    var code = await File.ReadAllTextAsync(path);

    var newDir = Path.Combine(solutionDirectory, "docs", "samples");
    Directory.CreateDirectory(newDir);
    
    var start = code.IndexOf("\n    {", StringComparison.Ordinal);
    var end = code.IndexOf("\n    }", StringComparison.Ordinal);
    code = code.Substring(start + 4, end - start + 4);
    
    var lines = code.Split('\n')[1..^2];
    code = string.Join('\n', lines.Select(x => x.Length > 8 ? x[8..] : string.Empty));
    
    var newPath = Path.Combine(newDir, $"{Path.GetExtension(Path.GetFileNameWithoutExtension(path)).TrimStart('.')}.md");
    await File.WriteAllTextAsync(newPath, $@"```csharp
{code}
```");
}

var mkDocs = await File.ReadAllTextAsync(mkDocsPath);
var newMkDocs = mkDocs.Replace(
    "# EXAMPLES #",
    $"- Examples:{string.Concat(Directory.EnumerateFiles(Path.Combine(solutionDirectory, "docs", "samples"), "*.md")
    .Select(x => $@"
  - {Path.GetFileNameWithoutExtension(x)}: samples/{Path.GetFileNameWithoutExtension(x)}.md"))}");
await File.WriteAllTextAsync(mkDocsPath, newMkDocs);

