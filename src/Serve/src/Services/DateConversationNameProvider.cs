using System.Globalization;
using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Services;

public class DateConversationNameProvider : IConversationNameProvider
{
    public Task<string> GetConversationName(IReadOnlyCollection<StoredMessage> messages)
    {
        return Task.FromResult(DateTime.Now.ToString("yy-MM-dd HH:mm", CultureInfo.InvariantCulture));
    }
}