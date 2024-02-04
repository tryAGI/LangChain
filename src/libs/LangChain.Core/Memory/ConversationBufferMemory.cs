using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Buffer for storing conversation memory.
/// 
/// NOTE: LangChain's buffer property is not implemented here
/// </summary>
public class ConversationBufferMemory : BaseChatMemory
{
    public MessageFormatter Formatter { get; set; } = new MessageFormatter();

    public string MemoryKey { get; set; } = "history";

    /// <inheritdoc />
    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    /// <summary>
    /// Initializes new buffered memory instance
    /// </summary>
    public ConversationBufferMemory()
        : base()
    {
    }

    /// <summary>
    /// Initializes new buffered memory instance with provided history store
    /// </summary>
    /// <param name="chatHistory">History backing store</param>
    public ConversationBufferMemory(BaseChatMessageHistory chatHistory)
        : base(chatHistory)
    {
    }

    /// <inheritdoc />
    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        string bufferText = Formatter.Format(ChatHistory.Messages);
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, bufferText } });
    }
}