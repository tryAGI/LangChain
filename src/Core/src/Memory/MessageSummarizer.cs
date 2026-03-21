using LangChain.Extensions;
using LangChain.Providers;
using Microsoft.Extensions.AI;
using static LangChain.Chains.Chain;

namespace LangChain.Memory;

public static class MessageSummarizer
{
    public const string SummaryPrompt = @"
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

    public static async Task<string> SummarizeAsync(
        this IChatClient chatClient,
        IEnumerable<Message> newMessages,
        string existingSummary,
        MessageFormatter? formatter = null,
        CancellationToken cancellationToken = default)
    {
        formatter ??= new MessageFormatter();
        var newLines = formatter.Format(newMessages);

        var chain =
            Set(existingSummary, outputKey: "summary")
            | Set(newLines, outputKey: "new_lines")
            | Template(SummaryPrompt)
            | LLM(chatClient);

        return await chain.RunAsync("text", cancellationToken: cancellationToken).ConfigureAwait(false) ?? string.Empty;
    }
}
