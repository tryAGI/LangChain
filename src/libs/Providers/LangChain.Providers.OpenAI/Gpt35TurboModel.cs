namespace LangChain.Providers;

/// <inheritdoc/>
public class Gpt35TurboModel : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// Most capable GPT-3.5 model and optimized for chat at 1/10th the cost of text-davinci-003. <br/>
    /// Will be updated with our latest model iteration 2 weeks after it is released. <br/>
    /// Max tokens: 4,096 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// <remarks>On June 27th, 2023, gpt-3.5-turbo will be updated to point from gpt-3.5-turbo-0301 to gpt-3.5-turbo-0613.</remarks>
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35TurboModel(string apiKey) : base(apiKey, id: ModelIds.Gpt35Turbo)
    {
    }

    #endregion
}