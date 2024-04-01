// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Interface for embedding models.
/// Provides functionality to create embeddings from input data using specific settings.
/// </summary>
public interface IEmbeddingModel : IModel<EmbeddingSettings>
{
    /// <summary>
    /// Gets the maximum length of input that the embedding model can process.
    /// </summary>
    public int MaximumInputLength { get; }

    /// <summary>
    /// Asynchronously creates embeddings based on the provided request and settings.
    /// </summary>
    /// <param name="request">The embedding request containing the input data for which embeddings are to be generated.</param>
    /// <param name="settings">Optional embedding settings to customize the embedding generation process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in an EmbeddingResponse object.</returns>
    Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default);
}