using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public class StringPromptValue : BasePromptValue
{
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; }


    public override IReadOnlyCollection<Message> ToChatMessages()
    {
        return new[] { Value.AsSystemMessage() };
    }

    public override string ToString() => Value;
}