using LangChain.Providers;

namespace LangChain.Memory;

/// <inheritdoc/>
public class ChatMessageHistory : BaseChatMessageHistory
{
    private readonly List<Message> _messages = new List<Message>();
    
    /// <inheritdoc/>
    public override IReadOnlyList<Message> Messages => _messages;

    /// <inheritdoc/>
    public override Task AddMessage(Message message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task Clear()
    {
        _messages.Clear();
        return Task.CompletedTask;
    }
}