using Azure;
using Azure.AI.OpenAI;
using System.Diagnostics;
using OpenAI;

namespace LangChain.Providers.Azure;

public class AzureOpenAiEmbeddingModel(
    AzureOpenAiProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    #region Properties

    /// <summary>
    /// API has limit of 2048 elements in array per request
    /// so we need to split texts into batches
    /// https://platform.openai.com/docs/api-reference/embeddings
    /// </summary>
    public int EmbeddingBatchSize { get; init; } = 2048;

    /// <inheritdoc/>
    public int MaximumInputLength => (int)(CreateEmbeddingRequestModelExtensions.ToEnum(Id)?.GetMaxInputTokens() ?? 0);

    #endregion

    #region Methods

    /// <inheritdoc/>
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var index = 0;
        var batches = new List<string[]>();
        while (index < request.Strings.Count)
        {
            batches.Add(request.Strings.Skip(index).Take(EmbeddingBatchSize).ToArray());
            index += EmbeddingBatchSize;
        }

        var results = await Task.WhenAll(batches.Select(async batch =>
        {
            var watch = Stopwatch.StartNew();
            var embeddingOptions = new EmbeddingsOptions(Id, batch);

            var response = await provider.Client.GetEmbeddingsAsync(embeddingOptions, cancellationToken).ConfigureAwait(false);

            var usage = GetUsage(response) with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            //provider.AddUsage(usage);

            return response.Value.Data
                .Select(x => x.Embedding.ToArray())
                .ToArray();
        })).ConfigureAwait(false);

        var rr = results
            .SelectMany(x => x.ToArray())
            .ToArray();

        return new EmbeddingResponse
        {
            Values = rr,
            Usage = Usage.Empty,
            UsedSettings = EmbeddingSettings.Default,
            Dimensions = rr.FirstOrDefault()?.Length ?? 0,
        };
    }

    private Usage GetUsage(Response<Embeddings>? response)
    {
        if (response?.Value?.Usage == null!)
        {
            return Usage.Empty;
        }

        var tokens = response.Value?.Usage.PromptTokens ?? 0;

        return Usage.Empty with
        {
            InputTokens = tokens,
        };
    }

    #endregion
}
