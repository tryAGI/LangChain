using System.Text.Json;
using LangChain.Chat;
using LangChain.Schema;

namespace LangChain.Prompts;

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

    public override BaseChatMessage[] ToChatMessages()
    {
        return this.Messages;
    }
}