namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="PromptTokens"></param>
/// <param name="CompletionTokens"></param>
/// <param name="Messages"></param>
/// <param name="PriceInUsd"></param>
public readonly partial record struct Usage(
    int PromptTokens = 0,
    int CompletionTokens = 0,
    int Messages = 0,
    double PriceInUsd = 0.0d)
{
    /// <summary>
    /// 
    /// </summary>
    public static Usage Empty { get; } = new(
        PromptTokens: 0,
        CompletionTokens: 0,
        Messages: 0,
        PriceInUsd: 0.0d);
    
    /// <summary>
    /// 
    /// </summary>
    public int TotalTokens => PromptTokens + CompletionTokens;
}