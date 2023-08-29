namespace LangChain.Providers.HuggingFace;

/// <inheritdoc cref="RecommendedModelIds.Gpt2" />
public class Gpt2Model : HuggingFaceModel
{
    #region Constructors

    /// <inheritdoc cref="RecommendedModelIds.Gpt2" />
    public Gpt2Model(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: RecommendedModelIds.Gpt2)
    {
    }

    #endregion
}