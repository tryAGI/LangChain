namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="Messages"></param>
/// <param name="Usage"></param>
public readonly record struct ChatResponse(
    IReadOnlyCollection<Message> Messages,
    Usage Usage)
{
    /// <summary>
    /// 
    /// </summary>
    public static ChatResponse Empty { get; } = new(
        Messages: Array.Empty<Message>(),
        Usage: Usage.Empty);
}