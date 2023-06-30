namespace LangChain.Providers;

/// <inheritdoc/>
public class Gpt4Model : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// More capable than any GPT-3.5 model, able to do more complex tasks, and optimized for chat. <br/>
    /// Will be updated with our latest model iteration 2 weeks after it is released. <br/>
    /// Max tokens: 8,192 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// <remarks>On June 27th, 2023, gpt-4 will be updated to point from gpt-4-0314 to gpt-4-0613, the latest model iteration.</remarks>
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt4Model(string apiKey) : base(apiKey, id: ModelIds.Gpt4)
    {
    }

    #endregion
}