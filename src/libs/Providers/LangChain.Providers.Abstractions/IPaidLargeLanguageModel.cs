namespace LangChain.Providers;

/// <summary>
/// Interface for models offering tokens for money.
/// </summary>
public interface IPaidLargeLanguageModel
{
    /// <summary>
    /// Returns the price for the given number of tokens in USD.
    /// </summary>
    /// <param name="promptTokens"></param>
    /// <param name="completionTokens"></param>
    /// <returns></returns>
    public double CalculatePriceInUsd(int promptTokens, int completionTokens);
}