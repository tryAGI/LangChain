using LLama;
using LLama.Common;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
public sealed class LLamaSharpEmbeddings
    : Model<EmbeddingSettings>, IEmbeddingModel, IDisposable
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
            Temperature = temperature,
            EmbeddingMode = true
        });
    }

    private readonly LLamaSharpConfiguration _configuration;
    private readonly LLamaWeights _model;
    private readonly LLamaEmbedder _embedder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public LLamaSharpEmbeddings(LLamaSharpConfiguration configuration) : base(id: "LLamaSharp")
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        var parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,
            Embeddings = configuration.EmbeddingMode
        };
        _model = LLamaWeights.LoadFromFile(parameters);
        _configuration = configuration;
        _embedder = new LLamaEmbedder(_model, parameters);
    }

    /// <inheritdoc />
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var values = await Task.WhenAll(request.Strings
            .Select(prompt => _embedder.GetEmbeddings(prompt, cancellationToken))
            .ToArray()).ConfigureAwait(false);

        return new EmbeddingResponse
        {
            Values = values,
            Usage = Usage.Empty,
            UsedSettings = EmbeddingSettings.Default,
            Dimensions = values.FirstOrDefault()?.Length ?? 0,
        };
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