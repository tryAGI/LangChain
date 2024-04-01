using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve.Interfaces;

public interface IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages);
}