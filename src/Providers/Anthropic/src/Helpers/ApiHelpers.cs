namespace LangChain.Providers.Anthropic;

#pragma warning disable CS0618

/// <summary>
/// </summary>
/// <summary>
/// </summary>
public static class ApiHelpers
{
    /// <summary>
    ///     According https://www-files.anthropic.com/production/images/model_pricing_july2023.pdf <br />
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="completionTokens"></param>
    /// <param name="promptTokens"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static double CalculatePriceInUsd(this CreateMessageRequestModel modelId, int completionTokens, int promptTokens)
    {
        var promptPricePerToken = modelId switch
        {
            CreateMessageRequestModel.Claude20 => 8.00,
            CreateMessageRequestModel.Claude21 => 8.00,
            CreateMessageRequestModel.Claude3Haiku20240307 => 0.25,
            CreateMessageRequestModel.Claude3Sonnet20240229 => 3.00,
            CreateMessageRequestModel.Claude3Opus20240229 => 15.00,
            CreateMessageRequestModel.ClaudeInstant12 => 0.80,

            _ => throw new NotImplementedException()
        } * 0.001 * 0.001;
        var completionPricePerToken = modelId switch
        {
            CreateMessageRequestModel.Claude20 => 24.0,
            CreateMessageRequestModel.Claude21 => 24.0,
            CreateMessageRequestModel.Claude3Haiku20240307 => 1.25,
            CreateMessageRequestModel.Claude3Sonnet20240229 => 5.00,
            CreateMessageRequestModel.Claude3Opus20240229 => 75.00,
            CreateMessageRequestModel.ClaudeInstant12 => 5.51,
            _ => throw new NotImplementedException()
        } * 0.001 * 0.001;

        return completionTokens * completionPricePerToken +
               promptTokens * promptPricePerToken;
    }

    /// <summary>
    ///     Calculates the maximum number of tokens possible to generate for a model. <br />
    ///     According https://docs.anthropic.com/claude/reference/selecting-a-model <br />
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int CalculateContextLength(this CreateMessageRequestModel modelId)
    {
        return modelId switch
        {
            CreateMessageRequestModel.Claude20 => 100000,
            CreateMessageRequestModel.ClaudeInstant12 => 100000,
            CreateMessageRequestModel.Claude21 => 200000,
            CreateMessageRequestModel.Claude3Haiku20240307 => 200000,
            CreateMessageRequestModel.Claude3Sonnet20240229 => 200000,
            CreateMessageRequestModel.Claude3Opus20240229 => 200000,
            CreateMessageRequestModel.Claude35Sonnet20240620 => 200000,
            _ => throw new NotImplementedException()
        };
    }
}