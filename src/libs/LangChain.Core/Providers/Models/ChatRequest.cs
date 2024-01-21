namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="Messages"></param>
/// <param name="StopSequences"></param>
public readonly record struct ChatRequest(
    IReadOnlyCollection<Message> Messages,
    IReadOnlyCollection<string>? StopSequences = null)
{
    /// <summary>
    /// 
    /// </summary>
    public static ChatRequest Empty { get; } = new(
        Messages: Array.Empty<Message>(),
        StopSequences: null);
    
    public static implicit operator ChatRequest(string message)
    {
        return ToChatRequest(message);
    }

    public static ChatRequest ToChatRequest(string message)
    {
        return new ChatRequest(
            Messages: new[] { message.AsSystemMessage() },
            StopSequences: null);
    }
}