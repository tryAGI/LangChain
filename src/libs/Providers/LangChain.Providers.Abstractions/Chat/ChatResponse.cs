// ReSharper disable once CheckNamespace
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace LangChain.Providers;

#pragma warning disable CA2225

/// <summary>
/// 
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// 
    /// </summary>
    public required IReadOnlyCollection<Message> Messages { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public required ChatSettings UsedSettings { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Usage Usage { get; init; } = Usage.Empty;
    
    /// <summary>
    /// Returns the last message content.
    /// </summary>
    public string LastMessageContent => Messages.LastOrDefault().Content ?? string.Empty;
    
    /// <inheritdoc />
    public override string ToString()
    {
        return LastMessageContent;
    }
    
    public void Deconstruct(
        out Message message,
        out Usage usage)
    {
        message = Messages.LastOrDefault();
        usage = Usage;
    }
    
    public void Deconstruct(
        out Message message,
        out Usage usage,
        out ChatSettings usedSettings)
    {
        message = Messages.LastOrDefault();
        usage = Usage;
        usedSettings = UsedSettings;
    }
    
    public static implicit operator Message[](ChatResponse response)
    {
        return response?.Messages.ToArray() ?? [];
    }
    
    public static implicit operator Message(ChatResponse response)
    {
        return response?.Messages.LastOrDefault() ?? Message.Empty;
    }
    
    public static implicit operator string(ChatResponse response)
    {
        return response?.LastMessageContent ?? string.Empty;
    }
    
    public static implicit operator Usage(ChatResponse response)
    {
        return response?.Usage ?? Usage.Empty;
    }
}