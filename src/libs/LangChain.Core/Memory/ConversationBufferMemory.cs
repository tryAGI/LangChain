using LangChain.Providers;
using System.Net.Mail;
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

    public IList<Message> BufferAsMessages => ChatHistory.Messages;

    public override List<string> MemoryVariables => new List<string> {MemoryKey};

    private string GetBufferString(
        IEnumerable<Message> messages)
    {
        List<string> stringMessages = new List<string>();

        foreach (var m in messages)
        {
            string role;
            switch (m.Role)
            {
                case MessageRole.Human:
                    role = HumanPrefix;
                    break;
                case MessageRole.Ai:
                    role = AiPrefix;
                    break;
                case MessageRole.System:
                    role = "System";
                    break;
                case MessageRole.FunctionCall:
                    role = "Function";
                    break;
                default:
                    throw new ArgumentException($"Unsupported message type: {m.GetType().Name}");
            }

            string message = $"{role}: {m.Content}";
            // TODO: Add special case for a function call

            stringMessages.Add(message);
        }

        return string.Join("\n", stringMessages);
    }


    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        return new OutputValues(new Dictionary<string, object> {{MemoryKey, BufferAsString}});
    }
}