namespace LangChain.Abstractions.Embeddings.Base;

/// <summary>
/// Interface for embedding models.
/// </summary>
/// <see cref="https://api.python.langchain.com/en/latest/embeddings/langchain.embeddings.base.Embeddings.html"/>
public interface IEmbeddings
{
    Task<float[][]> EmbedDocumentsAsync(
        string[] texts,
        CancellationToken cancellationToken = default);

    Task<float[]> EmbedQueryAsync(
        string text,
        CancellationToken cancellationToken = default);
}