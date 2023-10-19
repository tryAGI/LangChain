using LangChain.Schema;

namespace LangChain.Memory;

public abstract class BaseChatMemory : BaseMemory
{
    protected BaseChatMessageHistory ChatHistory { get; set; }
    public bool ReturnMessages { get; set; }

    public BaseChatMemory(BaseChatMemoryInput input)
    {
        if (input.ChatHistory is null) ChatHistory = new ChatMessageHistory();
        else ChatHistory = input.ChatHistory;
        ReturnMessages = input.ReturnMessages;
    }

    public abstract override OutputValues LoadMemoryVariables(InputValues inputValues);

    public override void SaveContext(InputValues inputValues, OutputValues outputValues)
    {
        ChatHistory.AddUserMessage(inputValues.Value[inputValues.Value.Keys.FirstOrDefault().ToString()].ToString());
        ChatHistory.AddAiMessage(outputValues.Value[outputValues.Value.Keys.FirstOrDefault().ToString()].ToString());
    }

    public void Clear()
    {
        ChatHistory.Clear();
    }
}