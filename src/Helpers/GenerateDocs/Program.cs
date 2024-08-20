var solutionDirectory = args.ElementAtOrDefault(0) ?? Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../.."));
var sampleDirectory = Path.Combine(solutionDirectory, "examples");
var mkDocsPath = Path.Combine(solutionDirectory, "mkdocs.yml");

var samplesDocDir = Path.Combine(solutionDirectory, "docs", "samples");
Directory.CreateDirectory(samplesDocDir);

File.Copy(
    Path.Combine(solutionDirectory, "README.md"),
    Path.Combine(solutionDirectory, "docs", "index.md"),
    overwrite: true);
File.Copy(
    Path.Combine(solutionDirectory, "src", "Cli", "README.md"),
    Path.Combine(solutionDirectory, "docs", "cli.md"),
    overwrite: true);

Console.WriteLine($"Generating samples from {sampleDirectory}...");
foreach (var path in Directory.EnumerateFiles(sampleDirectory, "Program.cs", SearchOption.AllDirectories))
{
    var folder = Path.GetFileName(Path.GetDirectoryName(path) ?? string.Empty)?.Replace("LangChain.Samples.", string.Empty);
    var code = await File.ReadAllTextAsync(path);

    var newPath = Path.Combine(samplesDocDir, $"{folder}.md");
    await File.WriteAllTextAsync(newPath, $@"```csharp
{code}
```");
}

var metaTestsFolder = Path.Combine(solutionDirectory, "src", "Meta", "test");
Console.WriteLine($"Generating samples from {metaTestsFolder}...");
foreach (var path in Directory.EnumerateFiles(metaTestsFolder, "WikiTests.*.cs", SearchOption.AllDirectories))
{
    var code = await File.ReadAllTextAsync(path);

    var lines = code.Split('\n').ToList();
    if (lines.All(x => string.IsNullOrWhiteSpace(x) || x.StartsWith("//")))
    {
        continue;
    }
    
    var usings = string.Join('\n', lines
        .Where(x => x.StartsWith("using"))
        .ToArray());
    
    var start = lines.IndexOf("    {");
    var end = lines.IndexOf("    }");
    lines = lines
        .GetRange(start + 1, end - start - 1)
        .Where(x => !x.Contains(".Should()"))
        .Select(x => x.StartsWith("        ") ? x[8..] : x)
        .ToList();
    
    const string commentPrefix = "//// ";
    var markdown = string.Empty;
    var completeCode = string.Join('\n', lines.Where(x => !x.StartsWith(commentPrefix)));
    bool isFirstCode = true;
    for (var i = 0; i < lines.Count;)
    {
        var startGroup = i;
        if (lines[i].StartsWith(commentPrefix))
        {
            while (i < lines.Count && lines[i].StartsWith(commentPrefix))
            {
                i++;
            }
            
            var comment = string.Join('\n', lines
                .GetRange(startGroup, i - startGroup)
                .Select(x => x[commentPrefix.Length..]));
            markdown += comment + '\n';
        }
        else
        {
            while (i < lines.Count && !lines[i].StartsWith(commentPrefix))
            {
                i++;
            }

            markdown += "```csharp";
            if (isFirstCode)
            {
                isFirstCode = false;
                markdown += Environment.NewLine + usings + Environment.NewLine;
            }
            
            markdown += $@"
{string.Join('\n', lines
    .GetRange(startGroup, i - startGroup)).Trim()}
```" + '\n';
        }
    }
    
    markdown += @$"
# Complete code

```csharp
{completeCode.Trim()}
```";

    var newPath = Path.Combine(samplesDocDir, $"{Path.GetExtension(Path.GetFileNameWithoutExtension(path)).TrimStart('.')}.md");
    await File.WriteAllTextAsync(newPath, markdown);
}

var mkDocs = await File.ReadAllTextAsync(mkDocsPath);
var newMkDocs = mkDocs.Replace(
    "# EXAMPLES #",
    $"- Examples:{string.Concat(Directory.EnumerateFiles(Path.Combine(solutionDirectory, "docs", "samples"), "*.md")
    .Select(x => $@"
  - {Path.GetFileNameWithoutExtension(x)}: samples/{Path.GetFileNameWithoutExtension(x)}.md"))}");
await File.WriteAllTextAsync(mkDocsPath, newMkDocs);

