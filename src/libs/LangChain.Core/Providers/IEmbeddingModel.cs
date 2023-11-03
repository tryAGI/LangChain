using LangChain.Abstractions.Embeddings.Base;

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IEmbeddingModel : IEmbeddings
{
    /// <summary>
    /// 
    /// </summary>
    public string EmbeddingModelId { get; }

    /// <summary>
    /// 
    /// </summary>
    public Usage TotalUsage { get; }

    /// <summary>
    /// 
    /// </summary>
    public int MaximumInputLength { get; }
}