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

    [Description("Reads the content of a file.")]
    Task<string> ReadContentAsync(
        [Description("The path of the file to read.")] string path,
        CancellationToken cancellationToken = default);

    [Description("Writes content to a file. Prompts for confirmation. Returns 'Cancelled.' if user cancels.")]
    Task<string> WriteContentAsync(
        [Description("The path of the file to write.")] string path,
        [Description("The content to write to the file.")] string newContent,
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

    public async Task<string> ReadContentAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Reading file at path: {path}");

        return await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> WriteContentAsync(
        string path,
        string newContent,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(@$"Are you sure you want to write to the file? Press Y to confirm, N to cancel.
Path: {path}
NewContent: {newContent}");
        while (Console.ReadKey() is var keyInfo)
        {
            if (keyInfo.Key is ConsoleKey.N)
            {
                Console.WriteLine("Cancelled.");
                return "Cancelled.";
            }
            if (keyInfo.Key is ConsoleKey.Y)
            {
                break;
            }
        }

        await File.WriteAllTextAsync(path, newContent, cancellationToken).ConfigureAwait(false);

        Console.WriteLine("File written.");

        return "File written.";
    }
}