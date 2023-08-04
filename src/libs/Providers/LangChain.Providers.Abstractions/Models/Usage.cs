namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="PromptTokens"></param>
/// <param name="CompletionTokens"></param>
/// <param name="Messages"></param>
/// <param name="Time"></param>
/// <param name="PriceInUsd"></param>
public readonly partial record struct Usage(
    int PromptTokens,
    int CompletionTokens,
    int Messages,
    TimeSpan Time,
    double PriceInUsd)
{
    /// <summary>
    /// 
    /// </summary>
    public static Usage Empty { get; } = new(
        PromptTokens: 0,
        CompletionTokens: 0,
        Messages: 0,
        Time: TimeSpan.Zero,
        PriceInUsd: 0.0d);
    
    /// <summary>
    /// 
    /// </summary>
    public int TotalTokens => PromptTokens + CompletionTokens;
}