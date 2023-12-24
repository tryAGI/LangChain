using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

/// <inheritdoc />
public class ConversationBufferMemory : BaseChatMemory
{
    /// <summary>
    /// 
    /// </summary>
    public string HumanPrefix { get; set; } = "Human";
    
    /// <summary>
    /// 
    /// </summary>
    public string AiPrefix { get; set; } = "AI";

    /// <summary>
    /// 
    /// </summary>
    public string MemoryKey { get; set; } = "history";

    /// <inheritdoc />
    public ConversationBufferMemory(BaseChatMessageHistory chatHistory) : base(chatHistory)
    {
        ChatHistory = chatHistory;
    }

    // note: buffer property can't be implemented because of Any type as return type

    /// <summary>
    /// 
    /// </summary>
    public string BufferAsString => GetBufferString(BufferAsMessages);

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Message> BufferAsMessages => ChatHistory.Messages;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, BufferAsString } });
    }
}