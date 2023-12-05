using LangChain.Schema;

namespace LangChain.Memory;

/// <inheritdoc/>
public abstract class BaseChatMemory(
    BaseChatMessageHistory chatHistory)
    : BaseMemory
{
    /// <summary>
    /// 
    /// </summary>
    protected BaseChatMessageHistory ChatHistory { get; set; } = chatHistory;

    /// <summary>
    /// 
    /// </summary>
    public string? OutputKey { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? InputKey { get; set; }
    
    // note: return type can't be implemented because of Any type as return type in Buffer property

    /// <summary>
    /// This used just to save user message as input and AI message as output
    /// </summary>
    /// <param name="inputValues"></param>
    /// <param name="outputValues"></param>
    public override async Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        inputValues = inputValues ?? throw new ArgumentNullException(nameof(inputValues));
        outputValues = outputValues ?? throw new ArgumentNullException(nameof(outputValues));
        
        await ChatHistory.AddUserMessage(inputValues.Value[inputValues.Value.Keys.FirstOrDefault()].ToString()).ConfigureAwait(false);
        await ChatHistory.AddAiMessage(outputValues.Value[outputValues.Value.Keys.FirstOrDefault()].ToString()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task Clear()
    {
        return ChatHistory.Clear();
    }
}