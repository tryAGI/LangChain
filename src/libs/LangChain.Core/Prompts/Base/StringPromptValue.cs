using LangChain.Chat;
using LangChain.Schema;

namespace LangChain.Prompts.Base;

public class StringPromptValue : BasePromptValue
{
    public string Value { get; set; }


    public override BaseChatMessage[] ToChatMessages()
    {
        return new[] { new HumanChatMessage(Value) };
    }

    public override string ToString() => Value;
}