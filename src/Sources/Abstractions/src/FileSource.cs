using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class FileSource : ISource
{
    /// <summary>
    /// 
    /// </summary>
    public required string FilePath { get; init; }

    public Encoding Encoding { get; init; } = Encoding.UTF8;

    public FileSource()
    {
    }

    [SetsRequiredMembers]
    public FileSource(string filePath)
    {
        FilePath = filePath;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var content = await File2.ReadAllTextAsync(FilePath, Encoding, cancellationToken).ConfigureAwait(false);

        // It makes sense for agents, but we need tests for this
        // if (AutoDetectEncoding)
        // {
        //     // todo: change this to a more robust solution
        //     // bruteforce encoding detection
        //     var encodings = new[] { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode };
        //     foreach (var encoding in encodings)
        //     {
        //         try
        //         {
        //             content = await File2.ReadAllTextAsync(FilePath, encoding, cancellationToken).ConfigureAwait(false);
        //             break;
        //         }
        //         catch (DecoderFallbackException)
        //         {
        //             continue;
        //         }
        //     }
        // }

        return [new Document(content, metadata: new Dictionary<string, object>
        {
            ["source"] = FilePath,
        })];
    }
}