namespace LangChain.Abstractions.Embeddings.Base;

/// <summary>
/// Interface for embedding models.
/// https://api.python.langchain.com/en/latest/embeddings/langchain.embeddings.base.Embeddings.html
/// </summary>
public interface IEmbeddings
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<float[][]> EmbedDocumentsAsync(
        string[] texts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<float[]> EmbedQueryAsync(
        string text,
        CancellationToken cancellationToken = default);
}