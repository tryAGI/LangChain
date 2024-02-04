using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Abstract base class for chat memory.
/// 
/// NOTE: LangChain's return_messages property is not implemented due to differences between Python and C#
/// </summary>
public abstract class BaseChatMemory : BaseMemory
{
    public BaseChatMessageHistory ChatHistory { get; }

    public string? OutputKey { get; set; }

    public string? InputKey { get; set; }

    protected BaseChatMemory()
    {
        ChatHistory = new ChatMessageHistory();
    }

    protected BaseChatMemory(BaseChatMessageHistory chatHistory)
    {
        ChatHistory = chatHistory ?? throw new ArgumentNullException(nameof(chatHistory));
    }

    /// <inheritdoc/>
    public override async Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        inputValues = inputValues ?? throw new ArgumentNullException(nameof(inputValues));
        outputValues = outputValues ?? throw new ArgumentNullException(nameof(outputValues));

        // If the InputKey is not specified, there must only be one input value
        var inputKey = InputKey ?? inputValues.Value.Keys.Single();
        if (inputKey is not null)
        {
            await ChatHistory.AddUserMessage(inputValues.Value[inputKey].ToString() ?? string.Empty).ConfigureAwait(false);
        }

        // If the OutputKey is not specified, there must only be one output value
        var outputKey = OutputKey ?? outputValues.Value.Keys.Single();
        if (outputKey is not null)
        {
            await ChatHistory.AddAiMessage(outputValues.Value[outputKey].ToString() ?? string.Empty).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task Clear()
    {
        await ChatHistory.Clear().ConfigureAwait(false);
    }
}