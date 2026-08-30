using System.Text.Json;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class ChatPromptValue(
    IReadOnlyCollection<ChatMessage> messages)
    : BasePromptValue
{
    /// <summary>
    ///
    /// </summary>
    public IReadOnlyCollection<ChatMessage> Messages { get; set; } = messages;

    /// <inheritdoc/>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this.Messages);
    }

    /// <inheritdoc/>
    public override IReadOnlyCollection<ChatMessage> ToChatMessages()
    {
        return this.Messages;
    }
}
