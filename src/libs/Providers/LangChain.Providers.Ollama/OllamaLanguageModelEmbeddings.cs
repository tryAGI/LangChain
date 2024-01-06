using LangChain.Providers;
using System.Diagnostics;
using LangChain.Abstractions.Embeddings.Base;

namespace OllamaTest;

public class OllamaLanguageModelEmbeddings : IEmbeddings
{
    private readonly string _modelName;
    public OllamaLanguageModelOptions Options { get; }
    private readonly OllamaApiClient _api;


    public OllamaLanguageModelEmbeddings(string modelName, string? url=null,  OllamaLanguageModelOptions options=null)
    {
        _modelName = modelName;
        Options = options;
        
        url ??= "http://localhost:11434";
        options ??= new OllamaLanguageModelOptions();
        Options = options;
        _api = new OllamaApiClient(url);
        
    }

    public Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        double[][] result = new double[texts.Length][];
        for (int i = 0; i < texts.Length; i++)
        {
            result[i] = _api.GenerateEmbeddings(new GenerateEmbeddingRequest(){Prompt = texts[i],Model = _modelName,Options = Options}).Result.Embedding;
        }
        var result2 = result.Select(x => x.Select(y => (float)y).ToArray()).ToArray();
        return Task.FromResult(result2);
    }

    public Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        double[] result = _api.GenerateEmbeddings(new GenerateEmbeddingRequest() { Prompt = text, Model = _modelName, Options = Options }).Result.Embedding;
        var result2 = result.Select(x => (float)x).ToArray();
        return Task.FromResult(result2);
        
    }
}