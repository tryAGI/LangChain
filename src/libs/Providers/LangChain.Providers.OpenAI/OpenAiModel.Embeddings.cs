using System.Diagnostics;
using OpenAI.Constants;
using OpenAI.Embeddings;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : IEmbeddingModel
{
    #region Properties

    /// <inheritdoc cref="OpenAiConfiguration.EmbeddingModelId"/>
    public string EmbeddingModelId { get; init; } = EmbeddingModel.Ada002;
    
    /// <summary>
    /// API has limit of 2048 elements in array per request
    /// so we need to split texts into batches
    /// https://platform.openai.com/docs/api-reference/embeddings
    /// </summary>
    public int EmbeddingBatchSize { get; init; } = 2048;

    /// <inheritdoc/>
    public int MaximumInputLength => ContextLengths.Get(EmbeddingModelId);

    #endregion

    #region Methods

    private Usage GetUsage(EmbeddingsResponse response)
    {
        if (response.Usage == null!)
        {
            return Usage.Empty;
        }
        
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
        texts = texts ?? throw new ArgumentNullException(nameof(texts));
        
        var watch = Stopwatch.StartNew();

        var index = 0;
        var batches = new List<string[]>();
        while (index < texts.Length)
        {
            batches.Add(texts.Skip(index).Take(EmbeddingBatchSize).ToArray());
            index += EmbeddingBatchSize;
        }
        
        var results = await Task.WhenAll(batches.Select(async batch =>
        {
            var response = await Api.EmbeddingsEndpoint.CreateEmbeddingAsync(
                request: new EmbeddingsRequest(
                    input: batch,
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
            
            return response.Data
                .Select(static x => x.Embedding
                    .Select(static x => (float)x)
                    .ToArray())
                .ToArray();
        })).ConfigureAwait(false);

        return results
            .SelectMany(x => x)
            .ToArray();
    }

    #endregion
}