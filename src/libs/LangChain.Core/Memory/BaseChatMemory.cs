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
    public override Task SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        ChatHistory.AddUserMessage(inputValues.Value[inputValues.Value.Keys.FirstOrDefault().ToString()].ToString());
        ChatHistory.AddAiMessage(outputValues.Value[outputValues.Value.Keys.FirstOrDefault().ToString()].ToString());
        return Task.CompletedTask;
    }

    public override Task Clear()
    {
        ChatHistory.Clear();
        return Task.CompletedTask;
    }
}