using LangChain.NET.Chat;

namespace LangChain.NET.Schema;

public abstract class BasePromptValue
{
    internal abstract BaseChatMessage[] ToChatMessages();
    
    public abstract override string ToString();
}