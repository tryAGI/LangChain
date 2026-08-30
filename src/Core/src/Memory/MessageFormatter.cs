using Microsoft.Extensions.AI;

namespace LangChain.Memory;

public class MessageFormatter
{
    public string HumanPrefix { get; set; } = "Human";
    public string AiPrefix { get; set; } = "AI";
    public string SystemPrefix { get; set; } = "System";
    public string FunctionCallPrefix { get; set; } = "Function";
    public string FunctionResultPrefix { get; set; } = "Result";
    public string ChatPrefix { get; set; } = "Chat";

    private string GetPrefix(ChatRole role)
    {
        if (role == ChatRole.System) return SystemPrefix;
        if (role == ChatRole.User) return HumanPrefix;
        if (role == ChatRole.Assistant) return AiPrefix;
        if (role == ChatRole.Tool) return FunctionResultPrefix;
        return ChatPrefix;
    }

    public string Format(ChatMessage message)
    {
        string messagePrefix = GetPrefix(message.Role);

        return string.IsNullOrEmpty(messagePrefix) ? (message.Text ?? string.Empty) : $"{messagePrefix}: {message.Text}";
    }

    public string Format(IEnumerable<ChatMessage> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        List<string> formattedMessages = new List<string>();

        foreach (ChatMessage message in messages)
        {
            formattedMessages.Add(Format(message));
        }

        return string.Join("\n", formattedMessages);
    }
}
