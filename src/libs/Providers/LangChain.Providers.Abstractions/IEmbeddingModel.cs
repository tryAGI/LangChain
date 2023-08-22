namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IEmbeddingModel
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<IReadOnlyCollection<double>>> EmbedDocumentsAsync(
        string[] texts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<double>> EmbedQueryAsync(
        string text,
        CancellationToken cancellationToken = default);
}