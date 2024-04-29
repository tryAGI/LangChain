using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Conversation summarizer to chat memory.
/// </summary>
public class ConversationSummaryMemory : BaseChatMemory
{
    public MessageFormatter Formatter { get; set; } = new MessageFormatter();

    public string MemoryKey { get; set; } = "history";

    /// <inheritdoc />
    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    private IChatModel Model { get; }
    private string SummaryText { get; set; } = string.Empty;

    /// <summary>
    /// Initializes new summarizing memory instance with provided model
    /// </summary>
    /// <param name="model">Model to use for summarization</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryMemory(IChatModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <summary>
    /// Initializes new summarizing memory instance with provided model and history store
    /// </summary>
    /// <param name="model">Model to use for summarization</param>
    /// <param name="chatHistory">History backing store</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryMemory(IChatModel model, BaseChatMessageHistory chatHistory)
        : base(chatHistory)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <inheritdoc />
    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        return new OutputValues(new Dictionary<string, object> { { MemoryKey, SummaryText } });
    }

    /// <inheritdoc />
    public override async Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        // Save non-summarized values to the history
        await base.SaveContext(inputValues, outputValues).ConfigureAwait(false);

        // Since we are in SaveContext, can assume there are at least two messages (human + ai)
        var newMessages = ChatHistory.Messages
            .Skip(ChatHistory.Messages.Count - 2)
            .Take(2);

        SummaryText = await Model.SummarizeAsync(newMessages, SummaryText).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task Clear()
    {
        await base.Clear().ConfigureAwait(false);
        SummaryText = string.Empty;
    }
}