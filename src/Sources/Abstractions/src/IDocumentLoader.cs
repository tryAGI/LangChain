namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public interface IDocumentLoader
{
    /// <summary>
    /// Loads documents from a data source.
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, CancellationToken cancellationToken = default);
}