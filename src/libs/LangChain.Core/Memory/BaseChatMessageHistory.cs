using LangChain.Providers;

namespace LangChain.Memory;

public abstract class BaseChatMessageHistory
{
    public IList<Message> Messages;

    public void AddUserMessage(string message)
    {
        AddMessage(message.AsHumanMessage());
    }

    public void AddAiMessage(string message)
    {
        AddMessage(message.AsAiMessage());
    }

    public abstract void AddMessage(Message message);

    public abstract void Clear();
}