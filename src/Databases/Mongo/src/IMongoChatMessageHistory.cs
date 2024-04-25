using LangChain.Providers;

namespace LangChain.Databases.Mongo;

public interface IMongoChatMessageHistory
{
    IReadOnlyList<Message> Messages { get; }

    Task AddMessage(Message message);
    Task Clear();
}