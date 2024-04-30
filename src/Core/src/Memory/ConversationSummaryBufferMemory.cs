using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Buffer with summarizer for storing conversation memory.
/// </summary>
public class ConversationSummaryBufferMemory : BaseChatMemory
{
    private IChatModelWithTokenCounting Model { get; }

    private string SummaryText { get; set; } = string.Empty;

    public MessageFormatter Formatter { get; set; } = new MessageFormatter();
    public string MemoryKey { get; set; } = "history";
    public int MaxTokenCount { get; set; } = 2000;

    /// <inheritdoc />
    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    /// <summary>
    /// Initializes new memory instance with provided model and a default history store
    /// </summary>
    /// <param name="model">Model to use for summarization</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryBufferMemory(IChatModelWithTokenCounting model)
        : base()
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <summary>
    /// Initializes new memory instance with provided model and history store
    /// </summary>
    /// <param name="model">Model to use for summarization</param>
    /// <param name="chatHistory">History backing store</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryBufferMemory(IChatModelWithTokenCounting model, BaseChatMessageHistory chatHistory)
        : base(chatHistory)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <inheritdoc />
    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        string bufferText = Formatter.Format(GetMessages());
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, bufferText } });
    }

    /// <inheritdoc />
    public override async Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        await base.SaveContext(inputValues, outputValues).ConfigureAwait(false);

        // Maintain max token size of messages
        await PruneMessages().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task Clear()
    {
        await base.Clear().ConfigureAwait(false);
        SummaryText = string.Empty;
    }

    /// <summary>
    /// Prune messages if they exceed the max token limit
    /// </summary>
    /// <returns></returns>
    private async Task PruneMessages()
    {
        List<Message> prunedMessages = new List<Message>();

        int tokenCount = Model.CountTokens(ChatHistory.Messages);
        if (tokenCount > MaxTokenCount)
        {
            Queue<Message> queue = new Queue<Message>(ChatHistory.Messages);

            while (tokenCount > MaxTokenCount)
            {
                Message prunedMessage = queue.Dequeue();
                prunedMessages.Add(prunedMessage);

                tokenCount = Model.CountTokens(queue);
            }

            SummaryText = await Model.SummarizeAsync(prunedMessages, SummaryText).ConfigureAwait(false);

            await ChatHistory.SetMessages(queue).ConfigureAwait(false);
        }
    }

    private List<Message> GetMessages()
    {
        List<Message> messages = new List<Message>
        {
            SummaryText.AsSystemMessage()
        };
        messages.AddRange(ChatHistory.Messages);

        return messages;
    }
}