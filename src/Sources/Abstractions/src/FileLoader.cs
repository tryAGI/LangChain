namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class FileLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));

        var content = await File2.ReadAllTextAsync(dataSource.Value!, dataSource.Encoding, cancellationToken).ConfigureAwait(false);

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
            ["encoding"] = dataSource.Encoding.EncodingName,
            ["path"] = dataSource.Value ?? string.Empty,
            ["type"] = dataSource.Type.ToString(),
        })];
    }
}