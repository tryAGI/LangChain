using LangChain.Abstractions.Embeddings.Base;
using LLama.Common;
using LLama;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
public sealed class LLamaSharpEmbeddings : IEmbeddings, IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="temperature"></param>
    /// <returns></returns>
    public static LLamaSharpEmbeddings FromPath(string path, float temperature = 0)
    {
        return new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = path,
            Temperature = temperature
        });
    }
    
    private readonly LLamaSharpConfiguration _configuration;
    private readonly LLamaWeights _model;
    private readonly LLamaEmbedder _embedder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public LLamaSharpEmbeddings(LLamaSharpConfiguration configuration)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        var parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,

        };
        _model = LLamaWeights.LoadFromFile(parameters);
        _configuration = configuration;
        _embedder = new LLamaEmbedder(_model, parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));
        
        float[][] result = new float[texts.Length][];
        for (int i = 0; i < texts.Length; i++)
        {
            result[i] = _embedder.GetEmbeddings(texts[i]);
        }
        return Task.FromResult(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_embedder.GetEmbeddings(text));
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _model.Dispose();
        _embedder.Dispose();
    }
}