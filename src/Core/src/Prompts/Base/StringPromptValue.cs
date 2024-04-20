using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public class StringPromptValue : BasePromptValue
{
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override IReadOnlyCollection<Message> ToChatMessages()
    {
        return new[] { Value.AsSystemMessage() };
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}