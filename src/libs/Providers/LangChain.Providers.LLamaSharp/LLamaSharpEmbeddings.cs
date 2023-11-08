using LangChain.Abstractions.Embeddings.Base;
using LLama.Common;
using LLama;

namespace LangChain.Providers.LLamaSharp;

public class LLamaSharpEmbeddings:IEmbeddings
{

    public static LLamaSharpEmbeddings FromPath(string path, float temperature = 0)
    {
        return new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = path,
            Temperature = temperature
        });

    }
    protected readonly LLamaSharpConfiguration _configuration;
    protected readonly LLamaWeights _model;
    protected readonly ModelParams _parameters;
    private readonly LLamaEmbedder _embedder;

    public LLamaSharpEmbeddings(LLamaSharpConfiguration configuration)
    {
        _parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,

        };
        _model = LLamaWeights.LoadFromFile(_parameters);
        _configuration = configuration;
        _embedder = new LLamaEmbedder(_model, _parameters);
    }

    public Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default)
    {
        float[][] result = new float[texts.Length][];
        for (int i = 0; i < texts.Length; i++)
        {
            result[i] = _embedder.GetEmbeddings(texts[i]);
        }
        return Task.FromResult(result);
    }

    public Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_embedder.GetEmbeddings(text));
    }
}