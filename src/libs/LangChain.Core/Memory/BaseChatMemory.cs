using LangChain.Schema;

namespace LangChain.Memory;

public abstract class BaseChatMemory : BaseMemory
{
    protected BaseChatMessageHistory ChatHistory { get; set; }
    public string? OutputKey { get; set; }
    public string? InputKey { get; set; }
    
    // note: return type can't be implemented because of Any type as return type in Buffer property

    protected BaseChatMemory(BaseChatMessageHistory chatHistory)
    {
        ChatHistory = chatHistory;
    }

    /// <summary>
    /// This used just to save user message as input and AI message as output
    /// </summary>
    /// <param name="inputValues"></param>
    /// <param name="outputValues"></param>
    public override async Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        inputValues = inputValues ?? throw new ArgumentNullException(nameof(inputValues));
        outputValues = outputValues ?? throw new ArgumentNullException(nameof(outputValues));
        
        await ChatHistory.AddUserMessage(inputValues.Value[inputValues.Value.Keys.FirstOrDefault().ToString()].ToString()).ConfigureAwait(false);
        await ChatHistory.AddAiMessage(outputValues.Value[outputValues.Value.Keys.FirstOrDefault().ToString()].ToString()).ConfigureAwait(false);
    }

    public override Task Clear()
    {
        return ChatHistory.Clear();
    }
}