using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public class StringPromptValue : BasePromptValue
{
    /// <summary>
    ///
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override IReadOnlyCollection<ChatMessage> ToChatMessages()
    {
        return [new ChatMessage(ChatRole.System, Value)];
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
