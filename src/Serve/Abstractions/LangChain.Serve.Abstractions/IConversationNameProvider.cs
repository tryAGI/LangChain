using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Abstractions;

public interface IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages);
}