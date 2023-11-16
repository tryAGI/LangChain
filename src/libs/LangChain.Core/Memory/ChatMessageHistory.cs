using LangChain.Providers;

namespace LangChain.Memory;

public class ChatMessageHistory : BaseChatMessageHistory
{
    public override Task AddMessage(Message message)
    {
        Messages.Add(message);
        return Task.CompletedTask;
    }

    public override Task Clear()
    {
        Messages.Clear();
        return Task.CompletedTask;
    }
}