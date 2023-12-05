using System.Text.Json;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class ChatPromptValue(
    IReadOnlyCollection<Message> messages)
    : BasePromptValue
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<Message> Messages { get; set; } = messages;

    /// <inheritdoc/>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this.Messages);
    }

    /// <inheritdoc/>
    public override IReadOnlyCollection<Message> ToChatMessages()
    {
        return this.Messages;
    }
}