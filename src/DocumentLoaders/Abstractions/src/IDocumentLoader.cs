namespace LangChain.DocumentLoaders;

/// <summary>
/// Defines a contract for loading documents from a data source.
/// </summary>
public interface IDocumentLoader
{
    /// <summary>
    /// Loads documents from a data source.
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyCollection<Document>> LoadAsync(
        DataSource dataSource,
        DocumentLoaderSettings? settings = null,
        CancellationToken cancellationToken = default);
}