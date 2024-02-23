using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;

namespace LangChain.Utilities.Services;

public class DateConversationNameProvider: IConversationNameProvider
{
    public Task<string> GetConversationName(List<StoredMessage> messages)
    {
        return Task.FromResult(DateTime.Now.ToString("yy-MM-dd HH:mm"));
    }
}