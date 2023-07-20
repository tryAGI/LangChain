namespace LangChain.Providers;

/// <inheritdoc/>
public class ClaudeModel : AnthropicModel
{
    #region Constructors

    /// <summary>
    /// Superior performance on tasks that require complex reasoning. <br/>
    /// Max tokens: 100,000 tokens <br/>
    /// Training data: Up to February 2023 <br/>
    /// </summary>
    public ClaudeModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.Claude)
    {
    }

    #endregion
}