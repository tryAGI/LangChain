using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Services;

public class CustomNameProvider(Func<List<StoredMessage>, Task<string>> generator) : IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages)
    {
        return generator(messages);
    }
}