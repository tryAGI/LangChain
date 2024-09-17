using LangChain.Providers;

namespace LangChain.Memory;

public class MessageFormatter
{
    public string HumanPrefix { get; set; } = "Human";
    public string AiPrefix { get; set; } = "AI";
    public string SystemPrefix { get; set; } = "System";
    public string FunctionCallPrefix { get; set; } = "Function";
    public string FunctionResultPrefix { get; set; } = "Result";
    public string ChatPrefix { get; set; } = "Chat";

    private string GetPrefix(MessageRole role)
    {
        switch (role)
        {
            case MessageRole.System:
                return SystemPrefix;

            case MessageRole.Human:
                return HumanPrefix;

            case MessageRole.Ai:
                return AiPrefix;

            case MessageRole.ToolCall:
                return FunctionCallPrefix;

            case MessageRole.ToolResult:
                return FunctionResultPrefix;

            case MessageRole.Chat:
                return ChatPrefix;

            default:
                throw new ArgumentException("Unrecognized message role", nameof(role));
        }
    }

    public string Format(Message message)
    {
        string messagePrefix = GetPrefix(message.Role);

        return string.IsNullOrEmpty(messagePrefix) ? message.Content : $"{messagePrefix}: {message.Content}";
    }

    public string Format(IEnumerable<Message> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        List<string> formattedMessages = new List<string>();

        foreach (Message message in messages)
        {
            formattedMessages.Add(Format(message));
        }

        return string.Join("\n", formattedMessages);
    }
}