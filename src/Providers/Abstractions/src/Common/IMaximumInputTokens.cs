namespace LangChain.Providers;

/// <summary>
/// Defined a model with maximum input tokens.
/// </summary>
public interface IMaximumInputTokens
{
    /// <summary>
    /// 
    /// </summary>
    public int MaximumInputTokens { get; }
}