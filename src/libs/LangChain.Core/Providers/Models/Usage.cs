namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="InputTokens"></param>
/// <param name="OutputTokens"></param>
/// <param name="Messages"></param>
/// <param name="Time"></param>
/// <param name="PriceInUsd"></param>
public readonly partial record struct Usage(
    int InputTokens,
    int OutputTokens,
    int Messages,
    TimeSpan Time,
    double PriceInUsd)
{
    /// <summary>
    /// 
    /// </summary>
    public static Usage Empty { get; } = new(
        InputTokens: 0,
        OutputTokens: 0,
        Messages: 0,
        Time: TimeSpan.Zero,
        PriceInUsd: 0.0d);

    /// <summary>
    /// 
    /// </summary>
    public int TotalTokens => InputTokens + OutputTokens;
}