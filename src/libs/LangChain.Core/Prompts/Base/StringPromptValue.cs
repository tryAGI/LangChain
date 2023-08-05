using LangChain.NET.Chat;
using LangChain.NET.Schema;

namespace LangChain.NET.Prompts.Base;

public class StringPromptValue : BasePromptValue
{
    public string Value { get; set; }


    internal override BaseChatMessage[] ToChatMessages()
    {
        return new[] { new HumanChatMessage(Value) };
    }

    public override string ToString() => Value;
}