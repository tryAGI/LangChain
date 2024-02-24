using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Buffer for storing conversation memory.
/// 
/// NOTE: LangChain's buffer property is not implemented here
/// </summary>
public class ConversationWindowBufferMemory : BaseChatMemory
{   
    public MessageFormatter Formatter { get; set; } = new MessageFormatter();

    public string MemoryKey { get; set; } = "history";

    /// <summary>
    /// Number of messages to store in buffer.
    /// 
    /// This is actually the number of Human+AI pairs of messages.
    /// This is the 'k' property in python langchain
    /// </summary>
    public int WindowSize { get; set; } = 5;

    /// <inheritdoc />
    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    /// <summary>
    /// Initializes new windowed buffer memory instance
    /// </summary>
    public ConversationWindowBufferMemory()
        : base()
    {
    }

    /// <summary>
    /// Initializes new windowed buffer memory instance with provided history store
    /// </summary>
    /// <param name="chatHistory">History backing store</param>
    public ConversationWindowBufferMemory(BaseChatMessageHistory chatHistory)
        : base(chatHistory)
    {
    }

    /// <inheritdoc />
    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        string bufferText = Formatter.Format(GetMessages());
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, bufferText } });
    }

    private List<Message> GetMessages()
    {
        int numMessages = Math.Min(ChatHistory.Messages.Count, WindowSize * 2);

        return ChatHistory.Messages
            .Skip(ChatHistory.Messages.Count - numMessages)
            .Take(numMessages)
            .ToList();
    }
}