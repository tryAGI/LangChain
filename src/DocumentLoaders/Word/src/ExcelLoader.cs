namespace LangChain.DocumentLoaders;

/// <summary>
/// 
/// </summary>
public sealed class ExcelLoader(bool firstRowIsHeader = true) : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(
        DataSource dataSource,
        DocumentLoaderSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        
        var markdowns = ExcelToMarkdown.Convert(stream, firstRowIsHeader);

        var metadata = settings.CollectMetadataIfRequired(dataSource);
        
        return markdowns
            .Select(x => new Document(x.Value, metadata: metadata?.With("Worksheet", x.Key)))
            .ToArray();
    }
}