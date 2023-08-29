namespace LangChain.Providers.Anthropic;

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class ClaudeInstantModel : AnthropicModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.ClaudeInstant" />
    public ClaudeInstantModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.ClaudeInstant)
    {
    }

    #endregion
}