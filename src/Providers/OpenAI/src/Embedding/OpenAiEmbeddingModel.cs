using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiEmbeddingModel(
    OpenAiProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    [CLSCompliant(false)]
    public OpenAiEmbeddingModel(
        OpenAiProvider provider,
        CreateEmbeddingRequestModel id)
        : this(provider, id.ToValueString())
    {
    }

    /// <summary>
    /// API has limit of 2048 elements in array per request
    /// so we need to split texts into batches
    /// https://platform.openai.com/docs/api-reference/embeddings
    /// </summary>
    public int EmbeddingBatchSize { get; init; } = 2048;

    /// <inheritdoc/>
    public int MaximumInputLength => (int)(CreateEmbeddingRequestModelExtensions.ToEnum(Id)?.GetMaxInputTokens() ?? 0);

    private Usage GetUsage(CreateEmbeddingResponse response)
    {
        var tokens = response.Usage.PromptTokens;
        var priceInUsd = CreateEmbeddingRequestModelExtensions.ToEnum(Id)?
            .GetPriceInUsd(tokens: tokens) ?? double.NaN;

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
            var response = await provider.Api.Embeddings.CreateEmbeddingAsync(
                input: batch,
                model: Id,
                encodingFormat: CreateEmbeddingRequestEncodingFormat.Float,
                dimensions: null,
                user: usedSettings.User!,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var usage = GetUsage(response) with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            provider.AddUsage(usage);

            return response.Data
                .Select(static x => x.Embedding1
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
            Dimensions = results.FirstOrDefault()?.FirstOrDefault()?.Length ?? 0,
        };
    }
}