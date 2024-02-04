using System.Diagnostics;
using OpenAI.Constants;
using OpenAI.Embeddings;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiEmbeddingModel(
    OpenAiProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    /// <summary>
    /// API has limit of 2048 elements in array per request
    /// so we need to split texts into batches
    /// https://platform.openai.com/docs/api-reference/embeddings
    /// </summary>
    public int EmbeddingBatchSize { get; init; } = 2048;

    /// <inheritdoc/>
    public int MaximumInputLength => EmbeddingModels.ById(Id)?.MaxInputTokens ?? 0;

    private Usage GetUsage(EmbeddingsResponse response)
    {
        if (response.Usage == null!)
        {
            return Usage.Empty;
        }
        
        var tokens = response.Usage.PromptTokens ?? 0;
        var priceInUsd = EmbeddingModels
            .ById(Id)?
            .GetPriceInUsd(tokens: tokens) ?? 0.0D;

        return Usage.Empty with
        {
            InputTokens = tokens,
            PriceInUsd = priceInUsd,
        };
    }

    /// <inheritdoc/>
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();

        var index = 0;
        var batches = new List<string[]>();
        while (index < request.Strings.Count)
        {
            batches.Add(request.Strings.Skip(index).Take(EmbeddingBatchSize).ToArray());
            index += EmbeddingBatchSize;
        }
        
        var usedSettings = OpenAiEmbeddingSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.EmbeddingSettings);
        var results = await Task.WhenAll(batches.Select(async batch =>
        {
            var response = await provider.Api.EmbeddingsEndpoint.CreateEmbeddingAsync(
                request: new EmbeddingsRequest(
                    input: batch,
                    model: Id,
                    user: usedSettings.User!),
                cancellationToken).ConfigureAwait(false);

            var usage = GetUsage(response) with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            provider.AddUsage(usage);
            
            return response.Data
                .Select(static x => x.Embedding
                    .Select(static x => (float)x)
                    .ToArray())
                .ToArray();
        })).ConfigureAwait(false);

        return new EmbeddingResponse
        {
            Values = results
                .SelectMany(x => x)
                .ToArray(),
            UsedSettings = usedSettings,
            Usage = Usage,
        };
    }
}