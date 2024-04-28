using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Services;

public class CustomNameProvider(Func<IReadOnlyCollection<StoredMessage>, Task<string>> generator) : IConversationNameProvider
{
    public Task<string> GetConversationName(IReadOnlyCollection<StoredMessage> messages)
    {
        return generator(messages);
    }
}