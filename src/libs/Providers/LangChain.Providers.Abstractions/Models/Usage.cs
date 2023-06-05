namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="PromptTokens"></param>
/// <param name="CompletionTokens"></param>
/// <param name="TotalTokens"></param>
/// <param name="Messages"></param>
/// <param name="PriceInUsd"></param>
public readonly record struct Usage(
    int PromptTokens = 0,
    int CompletionTokens = 0,
    int TotalTokens = 0,
    int Messages = 0,
    double PriceInUsd = 0.0d)
{
    /// <summary>
    /// 
    /// </summary>
    public static Usage Empty { get; } = new(
        PromptTokens: 0,
        CompletionTokens: 0,
        TotalTokens: 0,
        Messages: 0,
        PriceInUsd: 0.0d);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Usage operator +(Usage a, Usage b)
    {
        return new Usage(PromptTokens: a.PromptTokens + b.PromptTokens,
            CompletionTokens: a.CompletionTokens + b.CompletionTokens,
            TotalTokens: a.TotalTokens + b.TotalTokens,
            Messages: a.Messages + b.Messages,
            PriceInUsd: a.PriceInUsd + b.PriceInUsd);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Usage Add(Usage left, Usage right)
    {
        return left + right;
    }
}