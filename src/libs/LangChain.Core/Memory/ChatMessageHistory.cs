using LangChain.Providers;

namespace LangChain.Memory;

public class ChatMessageHistory : BaseChatMessageHistory
{
    private readonly List<Message> _messages = new List<Message>();
    public override IReadOnlyList<Message> Messages => _messages;

    public override Task AddMessage(Message message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public override Task Clear()
    {
        _messages.Clear();
        return Task.CompletedTask;
    }
}