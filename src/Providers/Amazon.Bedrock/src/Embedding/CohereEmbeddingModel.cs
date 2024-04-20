using System.Diagnostics;
using System.Text;
using System.Text.Json;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class CohereEmbeddingModel(
    BedrockProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    /// <summary>
    /// Creates embeddings for the input strings using the Cohere model.
    /// </summary>
    /// <param name="request">The `EmbeddingRequest` containing the input strings.</param>
    /// <param name="settings">Optional `EmbeddingSettings` to override the model's default settings.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An `EmbeddingResponse` containing the generated embeddings and usage information.</returns>
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();

        var usedSettings = CohereEmbeddingSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.EmbeddingSettings);

        var splitText = request.Strings.Split(chunkSize: (int)usedSettings.MaximumInputLength!);
        var embeddings = new List<float>(capacity: splitText.Count);

        var response = await provider.Api.InvokeModelAsync(Id, Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new
            {
                texts = splitText,
                input_type = "search_document"
            })
        ), cancellationToken).ConfigureAwait(false);

        embeddings.AddRange(response?["embeddings"]?[0]?
            .AsArray()
            .Select(x => (float?)x?.AsValue() ?? 0.0f)
            .ToArray() ?? []);

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new EmbeddingResponse
        {
            Values = embeddings
                .Select(f => new[] { f })
                .ToArray(),
            Usage = Usage.Empty,
            UsedSettings = usedSettings,
        };
    }
}