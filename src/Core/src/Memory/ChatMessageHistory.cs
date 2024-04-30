using LangChain.Providers;

namespace LangChain.Memory;

/// <summary>
/// In memory implementation of chat message history.
/// 
/// Stores messages in an in memory list.
/// </summary>
public class ChatMessageHistory : BaseChatMessageHistory
{
    private readonly List<Message> _messages = new List<Message>();

    /// <summary>
    /// Used to inspect and filter messages on their way to the history store
    /// NOTE: This is not a feature of python langchain
    /// </summary>
    public Predicate<Message> IsMessageAccepted { get; set; } = (x => true);

    /// <inheritdoc/>
    public override IReadOnlyList<Message> Messages => _messages;

    /// <inheritdoc/>
    public override Task AddMessage(Message message)
    {
        if (IsMessageAccepted(message))
        {
            _messages.Add(message);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task Clear()
    {
        _messages.Clear();
        return Task.CompletedTask;
    }
}