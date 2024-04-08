// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Interface for embedding models.
/// </summary>
public interface IEmbeddingModel : IModel<EmbeddingSettings>
{
    /// <summary>
    /// 
    /// </summary>
    public int MaximumInputLength { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default);
}