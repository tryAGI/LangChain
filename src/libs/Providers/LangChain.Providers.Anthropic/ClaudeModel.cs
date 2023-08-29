namespace LangChain.Providers.Anthropic;

/// <inheritdoc cref="ModelIds.Claude" />
public class ClaudeModel : AnthropicModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Claude" />
    public ClaudeModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.Claude)
    {
    }

    #endregion
}