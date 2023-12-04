using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

public class ConversationBufferMemory : BaseChatMemory
{
    public string HumanPrefix { get; set; } = "Human";
    public string AiPrefix { get; set; } = "AI";

    public string MemoryKey { get; set; } = "history";

    public ConversationBufferMemory(BaseChatMessageHistory chatHistory) : base(chatHistory)
    {
        ChatHistory = chatHistory;
    }

    // note: buffer property can't be implemented because of Any type as return type

    public string BufferAsString => GetBufferString(BufferAsMessages);

    public IReadOnlyList<Message> BufferAsMessages => ChatHistory.Messages;

    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    private string GetBufferString(IEnumerable<Message> messages)
    {
        var stringMessages = new List<string>();

        foreach (var m in messages)
        {
            string role = m.Role switch
            {
                MessageRole.Human => HumanPrefix,
                MessageRole.Ai => AiPrefix,
                MessageRole.System => "System",
                MessageRole.FunctionCall => "Function",
                _ => throw new ArgumentException($"Unsupported message type: {m.GetType().Name}")
            };

            string message = $"{role}: {m.Content}";
            // TODO: Add special case for a function call

            stringMessages.Add(message);
        }

        return string.Join("\n", stringMessages);
    }

    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, BufferAsString } });
    }
}