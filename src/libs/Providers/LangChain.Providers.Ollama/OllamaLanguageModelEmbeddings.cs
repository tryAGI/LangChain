using LangChain.Abstractions.Embeddings.Base;

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OllamaLanguageModelEmbeddings : IEmbeddings
{
    private readonly string _modelName;
    private readonly OllamaApiClient _api;
    
    /// <summary>
    /// 
    /// </summary>
    public OllamaLanguageModelOptions Options { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="url"></param>
    /// <param name="options"></param>
    public OllamaLanguageModelEmbeddings(
        string modelName,
        string? url = null,
        OllamaLanguageModelOptions? options = null)
    {
        _modelName = modelName;
        Options = options ?? new OllamaLanguageModelOptions();
        
        _api = new OllamaApiClient(url ?? "http://localhost:11434");
    }

    /// <inheritdoc />
    public async Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        double[][] result = new double[texts.Length][];
        for (int i = 0; i < texts.Length; i++)
        {
            var embeddings = await _api.GenerateEmbeddings(new GenerateEmbeddingRequest
            {
                Prompt = texts[i],
                Model = _modelName,Options = Options,
            }).ConfigureAwait(false);
            result[i] = embeddings.Embedding;
        }
        var result2 = result.Select(x => x.Select(y => (float)y).ToArray()).ToArray();
        return result2;
    }

    /// <inheritdoc />
    public async Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        double[] result = (await _api.GenerateEmbeddings(new GenerateEmbeddingRequest
        {
            Prompt = text,
            Model = _modelName,
            Options = Options
        }).ConfigureAwait(false)).Embedding;
        var result2 = result.Select(x => (float)x).ToArray();
        return result2;
    }
}