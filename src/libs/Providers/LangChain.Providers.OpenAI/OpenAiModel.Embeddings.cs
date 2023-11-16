using System.Diagnostics;
using OpenAI.Constants;
using OpenAI.Embeddings;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : IEmbeddingModel
{
    #region Properties

    /// <inheritdoc cref="OpenAiConfiguration.EmbeddingModelId"/>
    public string EmbeddingModelId { get; init; } = EmbeddingModel.Ada002;

    /// <inheritdoc/>
    public int MaximumInputLength => ContextLengths.Get(EmbeddingModelId);

    #endregion

    #region Methods

    private Usage GetUsage(EmbeddingsResponse response)
    {
        var tokens = response.Usage.PromptTokens ?? 0;
        var priceInUsd = EmbeddingPrices.TryGet(
            model: new EmbeddingModel(EmbeddingModelId),
            tokens: tokens) ?? 0.0D;

        return Usage.Empty with
        {
            InputTokens = tokens,
            PriceInUsd = priceInUsd,
        };
    }

    /// <inheritdoc/>
    public async Task<float[]> EmbedQueryAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        var response = await Api.EmbeddingsEndpoint.CreateEmbeddingAsync(
            request: new EmbeddingsRequest(
                input: text,
                model: EmbeddingModelId,
                user: User),
            cancellationToken).ConfigureAwait(false);

        var usage = GetUsage(response) with
        {
            Time = watch.Elapsed,
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return response.Data[0].Embedding.Select(static x => (float)x).ToArray();
    }

    /// <inheritdoc/>
    public async Task<float[][]> EmbedDocumentsAsync(
        string[] texts,
        CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(
            texts
                .Select(text => EmbedQueryAsync(text, cancellationToken))).ConfigureAwait(false);
    }

    #endregion
}