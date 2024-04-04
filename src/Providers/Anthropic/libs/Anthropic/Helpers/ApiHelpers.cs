using Anthropic.SDK.Constants;

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
    public static double CalculatePriceInUsd(string modelId, int completionTokens, int promptTokens)
    {
        var promptPricePerToken = modelId switch
        {
            AnthropicModels.Claude_v2 => 8.00,
            AnthropicModels.Claude_v2_1 => 8.00,
            AnthropicModels.Claude3Haiku => 0.25,
            AnthropicModels.Claude3Sonnet => 3.00,
            AnthropicModels.Claude3Opus => 15.00,
            AnthropicModels.ClaudeInstant_v1_2 => 0.80,

            _ => throw new NotImplementedException()
        } * 0.001 * 0.001;
        var completionPricePerToken = modelId switch
        {
            AnthropicModels.Claude_v2 => 24.0,
            AnthropicModels.Claude_v2_1 => 24.0,
            AnthropicModels.Claude3Haiku => 1.25,
            AnthropicModels.Claude3Sonnet => 5.00,
            AnthropicModels.Claude3Opus => 75.00,
            AnthropicModels.ClaudeInstant_v1_2 => 5.51,
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
    public static int CalculateContextLength(string modelId)
    {
        return modelId switch
        {
            AnthropicModels.Claude_v2 => 100000,
            AnthropicModels.ClaudeInstant_v1_2 => 100000,
            AnthropicModels.Claude_v2_1 => 200000,
            AnthropicModels.Claude3Haiku => 200000,
            AnthropicModels.Claude3Sonnet => 200000,
            AnthropicModels.Claude3Opus => 200000,
            _ => throw new NotImplementedException()
        };
    }
}