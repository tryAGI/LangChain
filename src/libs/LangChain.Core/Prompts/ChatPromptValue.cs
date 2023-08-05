using System.Text.Json;
using LangChain.NET.Chat;
using LangChain.NET.Schema;

namespace LangChain.NET.Prompts;

public class ChatPromptValue : BasePromptValue
{
    public BaseChatMessage[] Messages { get; set; }

    public ChatPromptValue(BaseChatMessage[] messages)
    {
        this.Messages = messages;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this.Messages);
    }

    internal override BaseChatMessage[] ToChatMessages()
    {
        return this.Messages;
    }
}