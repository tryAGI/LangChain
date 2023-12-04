using LangChain.Providers;

namespace LangChain.Memory;

public abstract class BaseChatMessageHistory
{
    public async Task AddUserMessage(string message)
    {
        await AddMessage(message.AsHumanMessage());
    }

    public async Task AddAiMessage(string message)
    {
        await AddMessage(message.AsAiMessage());
    }

    public abstract IReadOnlyList<Message> Messages { get; }

    public abstract Task AddMessage(Message message);

    public abstract Task Clear();
}