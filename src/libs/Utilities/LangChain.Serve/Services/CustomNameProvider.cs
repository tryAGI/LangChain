using LangChain.Chains.HelperChains;
using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;

namespace LangChain.Utilities.Services;

public class CustomNameProvider(Func<List<StoredMessage>, Task<string>> generator) : IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages)
    {
        return generator(messages);
    }
}