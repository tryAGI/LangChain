using LangChain.Serve.Interfaces;
using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve.Services;

public class CustomNameProvider(Func<List<StoredMessage>, Task<string>> generator) : IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages)
    {
        return generator(messages);
    }
}