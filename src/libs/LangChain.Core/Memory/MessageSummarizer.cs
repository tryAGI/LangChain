using LangChain.Providers;
using static LangChain.Chains.Chain;

namespace LangChain.Memory;

public class MessageSummarizer
{
    private const string SummaryPrompt = @"
Progressively summarize the lines of conversation provided, adding onto the previous summary returning a new summary.

EXAMPLE
Current summary:
The human asks what the AI thinks of artificial intelligence.The AI thinks artificial intelligence is a force for good.

New lines of conversation:
Human: Why do you think artificial intelligence is a force for good?
AI: Because artificial intelligence will help humans reach their full potential.

New summary:
The human asks what the AI thinks of artificial intelligence. The AI thinks artificial intelligence is a force for good because it will help humans reach their full potential.
END OF EXAMPLE

Current summary:
{summary}

 New lines of conversation:
{new_lines}

New summary:";

    private IChatModel Model { get; }
    private MessageFormatter Formatter { get; }

    public MessageSummarizer(IChatModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Formatter = new MessageFormatter();
    }

    public MessageSummarizer(IChatModel model, MessageFormatter formatter)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public async Task<string> Summarize(IEnumerable<Message> newMessages, string existingSummary)
    {
        string newLines = Formatter.Format(newMessages);

        var chain =
            Set(existingSummary, outputKey: "summary")
            | Set(newLines, outputKey: "new_lines")
            | Template(SummaryPrompt)
            | LLM(Model);

        return await chain.Run("text").ConfigureAwait(false);
    }
}