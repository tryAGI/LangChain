namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public static class OpenAiModelHelpers
{
    /// <summary>
    /// According https://openai.com/pricing/
    /// gpt-4	        $0.03 / 1K tokens	$0.06 / 1K tokens
    /// gpt-4-32	    $0.06 / 1K tokens	$0.12 / 1K tokens
    /// gpt-3.5-turbo	$0.002 / 1K tokens
    /// </summary>
    /// <param name="modelId">The model id we want to know the context size for.</param>
    /// <param name="completionTokens"></param>
    /// <param name="promptTokens"></param>
    /// <returns>The maximum context size</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static double CalculatePriceInUsd(string modelId, int completionTokens, int promptTokens)
    {
        var promptPricePerToken = modelId switch
        {
            "gpt-4" => 0.03 * 0.001,
            "gpt-4-32" => 0.06 * 0.001,
            "gpt-3.5-turbo" => 0.002 * 0.001,
            _ => throw new NotImplementedException(),
        };
        var completionPricePerToken = modelId switch
        {
            "gpt-4" => 0.06 * 0.001,
            "gpt-4-32" => 0.12 * 0.001,
            "gpt-3.5-turbo" => 0.002 * 0.001,
            _ => throw new NotImplementedException(),
        };
        
        return completionTokens * completionPricePerToken +
               promptTokens * promptPricePerToken;
    }

    /// <summary>
    /// Calculates the maximum number of tokens possible to generate for a model.
    /// According https://platform.openai.com/docs/models/overview
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int CalculateContextLength(string modelId)
    {
        return modelId switch
        {
            "gpt-4" or "gpt-4-0314" => 8_192,
            "gpt-4-32" or "gpt-4-32k-0314" => 32_768,
            "gpt-3.5-turbo" or "gpt-3.5-turbo-0301" => 4_096,
            
            "ada" or "text-ada-001" => 2_049,
            "text-babbage-001" => 2_040,
            "babbage" => 2_049,
            "text-curie-001" => 2_049,
            "curie" => 2_049,
            "davinci" => 2_049,
            "text-davinci-003" => 4_097,
            "text-davinci-002" => 4_097,
            "code-davinci-002" => 8_001,
            "code-davinci-001" => 8_001,
            "code-cushman-002" => 2_048,
            "code-cushman-001" => 2_048,
            
            _ => throw new NotImplementedException(),
        };
    }
}