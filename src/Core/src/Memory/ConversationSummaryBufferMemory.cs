using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Memory;

/// <summary>
/// Buffer with summarizer for storing conversation memory.
/// </summary>
public class ConversationSummaryBufferMemory : BaseChatMemory
{
    private IChatClient ChatClient { get; }

    private Func<IEnumerable<ChatMessage>, int>? TokenCounter { get; }

    private string SummaryText { get; set; } = string.Empty;

    public MessageFormatter Formatter { get; set; } = new MessageFormatter();
    public string MemoryKey { get; set; } = "history";
    public int MaxTokenCount { get; set; } = 2000;

    /// <inheritdoc />
    public override List<string> MemoryVariables => new List<string> { MemoryKey };

    /// <summary>
    /// Initializes new memory instance with provided chat client and a default history store.
    /// Uses a simple character-based token estimate (chars / 4) unless a custom tokenCounter is provided.
    /// </summary>
    /// <param name="chatClient">Chat client to use for summarization</param>
    /// <param name="tokenCounter">Optional custom token counter function</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryBufferMemory(IChatClient chatClient, Func<IEnumerable<ChatMessage>, int>? tokenCounter = null)
        : base()
    {
        ChatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        TokenCounter = tokenCounter;
    }

    /// <summary>
    /// Initializes new memory instance with provided chat client and history store
    /// </summary>
    /// <param name="chatClient">Chat client to use for summarization</param>
    /// <param name="chatHistory">History backing store</param>
    /// <param name="tokenCounter">Optional custom token counter function</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConversationSummaryBufferMemory(IChatClient chatClient, BaseChatMessageHistory chatHistory, Func<IEnumerable<ChatMessage>, int>? tokenCounter = null)
        : base(chatHistory)
    {
        ChatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        TokenCounter = tokenCounter;
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

    private int CountTokens(IEnumerable<ChatMessage> messages)
    {
        if (TokenCounter != null)
        {
            return TokenCounter(messages);
        }

        // Simple character-based estimate: ~4 chars per token
        return messages.Sum(m => (m.Text?.Length ?? 0)) / 4;
    }

    /// <summary>
    /// Prune messages if they exceed the max token limit
    /// </summary>
    /// <returns></returns>
    private async Task PruneMessages()
    {
        List<ChatMessage> prunedMessages = new List<ChatMessage>();

        int tokenCount = CountTokens(ChatHistory.Messages.ToChatMessages());
        if (tokenCount > MaxTokenCount)
        {
            Queue<ChatMessage> queue = new Queue<ChatMessage>(ChatHistory.Messages.ToChatMessages());

            while (tokenCount > MaxTokenCount)
            {
                ChatMessage prunedMessage = queue.Dequeue();
                prunedMessages.Add(prunedMessage);

                tokenCount = CountTokens(queue);
            }

            SummaryText = await ChatClient.SummarizeAsync(prunedMessages, SummaryText).ConfigureAwait(false);

            await ChatHistory.SetMessages(queue.ToLangChainMessages()).ConfigureAwait(false);
        }
    }

    private List<ChatMessage> GetMessages()
    {
        List<ChatMessage> messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, SummaryText)
        };
        messages.AddRange(ChatHistory.Messages.ToChatMessages());

        return messages;
    }
}
