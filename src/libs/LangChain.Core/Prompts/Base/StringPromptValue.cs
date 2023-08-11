using LangChain.Chat;
using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public class StringPromptValue : BasePromptValue
{
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; }


    public override BaseChatMessage[] ToChatMessages()
    {
        return new[] { new HumanChatMessage(Value) };
    }

    public override string ToString() => Value;
}