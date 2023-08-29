using System.Text.Json;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public class ChatPromptValue : BasePromptValue
{
    public IReadOnlyCollection<Message> Messages { get; set; }

    public ChatPromptValue(IReadOnlyCollection<Message> messages)
    {
        this.Messages = messages;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this.Messages);
    }

    public override IReadOnlyCollection<Message> ToChatMessages()
    {
        return this.Messages;
    }
}