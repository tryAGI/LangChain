namespace LangChain.Providers;

/// <inheritdoc/>
public class ClaudeInstantModel : AnthropicModel
{
    #region Constructors

    /// <summary>
    /// Low-latency, high throughout. <br/>
    /// Max tokens: 100,000 tokens <br/>
    /// Training data: Up to February 2023 <br/>
    /// </summary>
    public ClaudeInstantModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.ClaudeInstant)
    {
    }

    #endregion
}