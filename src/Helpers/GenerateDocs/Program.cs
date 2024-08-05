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

var mkDocs = await File.ReadAllTextAsync(mkDocsPath);
var newMkDocs = mkDocs.Replace(
    "# EXAMPLES #",
    $"- Examples:{string.Concat(Directory.EnumerateFiles(Path.Combine(solutionDirectory, "docs", "samples"), "*.md")
    .Select(x => $@"
  - {Path.GetFileNameWithoutExtension(x)}: samples/{Path.GetFileNameWithoutExtension(x)}.md"))}");
await File.WriteAllTextAsync(mkDocsPath, newMkDocs);

