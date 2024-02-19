namespace LangChain.Providers;

/// <summary>
/// Interface for models offering tokens for money.
/// </summary>
public interface IPaidLargeLanguageModel
{
    /// <summary>
    /// Returns the price for the given number of tokens in USD.
    /// </summary>
    /// <param name="inputTokens"></param>
    /// <param name="outputTokens"></param>
    /// <returns></returns>
    public double CalculatePriceInUsd(int inputTokens, int outputTokens);
}