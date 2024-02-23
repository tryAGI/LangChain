namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class OllamaEmbeddingModel(
    OllamaProvider provider,
    string id = "ollama")
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    /// <summary>
    /// Provider of the model.
    /// </summary>
    public OllamaProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    public int MaximumInputLength => 0;

    /// <inheritdoc />
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var results = new List<double[]>(capacity: request.Strings.Count);
        foreach (var prompt in request.Strings)
        {
            var response = await Provider.Api.GenerateEmbeddings(new GenerateEmbeddingRequest
            {
                Prompt = prompt,
                Model = Id,
                Options = Provider.Options,
            }).ConfigureAwait(false);
            
            results.Add(response.Embedding);
        }
        
        return new EmbeddingResponse
        {
            Values = results
                .Select(x => x.Select(y => (float)y).ToArray())
                .ToArray(),
            UsedSettings = EmbeddingSettings.Default,
        };
    }
}