using System.ComponentModel;
using CSharpToJsonSchema;

namespace LangChain.Cli.Commands;

[GenerateJsonSchema]
#pragma warning disable CA1515
public partial interface IFileSystemService
#pragma warning restore CA1515
{
    [Description("Finds file paths by content.")]
    public Task<IList<string>> FindFilePathsByContentAsync(
        [Description("The directory in which the search will be performed. Includes all subdirectories")] string directory,
        [Description("The content to search for in the files. Ignores case.")] string content,
        CancellationToken cancellationToken = default);
}

internal sealed class FileSystemService : IFileSystemService
{
    public async Task<IList<string>> FindFilePathsByContentAsync(
        string directory,
        string content,
        CancellationToken cancellationToken = default)
    {
        var paths = new List<string>();

        Console.WriteLine($"Searching for files in \"{directory}\" containing \"{content}\"...");

        foreach (var path in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
        {
            try
            {
                var extension = Path.GetExtension(path);
                if (extension is not ".txt" and not ".md" and not ".json" and not ".cs" and not ".csproj" and not ".sln" and not ".sh" and not ".yml" and not ".yaml")
                {
                    continue;
                }

                //FileInfo info = new FileInfo(path);
                var text = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);

                if (text.Contains(content, StringComparison.OrdinalIgnoreCase))
                {
                    paths.Add(path);
                }
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // ignore
            }
        }

        Console.WriteLine($"Found {paths.Count} files:");
        foreach (var path in paths)
        {
            Console.WriteLine(path);
        }

        return paths;
    }
}