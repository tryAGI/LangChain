namespace LangChain.Providers;

/// <summary>
/// Tracks input/output tokens, time, and cost.
/// </summary>
public readonly record struct Usage(
    int InputTokens,
    int OutputTokens,
    int Messages,
    TimeSpan Time,
    double? PriceInUsd)
{
    /// <summary>
    /// Empty usage.
    /// </summary>
    public static Usage Empty { get; } = new(
        InputTokens: 0,
        OutputTokens: 0,
        Messages: 0,
        Time: TimeSpan.Zero,
        PriceInUsd: null);

    /// <summary>
    /// Total tokens (input + output).
    /// </summary>
    public int TotalTokens => InputTokens + OutputTokens;
}
