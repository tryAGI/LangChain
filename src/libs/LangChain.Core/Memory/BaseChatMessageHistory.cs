using LangChain.Chat;

namespace LangChain.Memory;

public abstract class BaseChatMessageHistory
{
    public IList<BaseChatMessage> Messages;

    public void AddUserMessage(string message)
    {
        AddMessage(new HumanChatMessage(message));
    }

    public void AddAiMessage(string message)
    {
        AddMessage(new AiChatMessage(message));
    }

    public abstract void AddMessage(BaseChatMessage message);

    public abstract void Clear();
}