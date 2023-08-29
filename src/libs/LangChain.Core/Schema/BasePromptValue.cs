using LangChain.Providers;

namespace LangChain.Schema;

public abstract class BasePromptValue
{
    public abstract IReadOnlyCollection<Message> ToChatMessages();

    public abstract override string ToString();
}