using LangChain.Providers;

namespace LangChain.Memory;

public class ChatMessageHistory : BaseChatMessageHistory
{
    public override void AddMessage(Message message)
    {
        Messages.Add(message);
    }

    public override void Clear()
    {
        Messages.Clear();
    }
}