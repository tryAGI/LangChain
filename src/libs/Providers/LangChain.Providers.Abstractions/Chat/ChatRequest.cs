// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for chat requests.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Defines the messages for the request.
    /// </summary>
    public required IReadOnlyCollection<Message> Messages { get; init; }
    
    /// <inheritdoc cref="ToChatRequest(string)"/>
    public static implicit operator ChatRequest(string message)
    {
        return ToChatRequest(message);
    }
    
    /// <inheritdoc cref="ToChatRequest(Message)"/>
    public static implicit operator ChatRequest(Message message)
    {
        return ToChatRequest(message);
    }
    
    /// <inheritdoc cref="ToChatRequest(string)"/>
    public static implicit operator ChatRequest(Message[] messages)
    {
        return ToChatRequest(messages);
    }
    
    /// <summary>
    /// Explicitly converts a string to a <see cref="ChatRequest"/>. <br/>
    /// Will be converted to a <see cref="ChatRequest"/>
    /// with a single <see cref="Message"/> of <see cref="MessageRole.System"/>
    /// with the content of the string.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChatRequest ToChatRequest(string message)
    {
        return new ChatRequest
        {
            Messages = new[] { message.AsSystemMessage() },
        };
    }
    
    /// <summary>
    /// Explicitly converts a string to a <see cref="ChatRequest"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChatRequest ToChatRequest(Message message)
    {
        return new ChatRequest
        {
            Messages = [message],
        };
    }
    
    /// <summary>
    /// Explicitly converts a string to a <see cref="ChatRequest"/>.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public static ChatRequest ToChatRequest(IReadOnlyCollection<Message> messages)
    {
        return new ChatRequest
        {
            Messages = messages,
        };
    }
}