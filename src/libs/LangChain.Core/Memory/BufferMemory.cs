using LangChain.Schema;

namespace LangChain.Memory;

public class BufferMemory : BaseChatMemory
{
    public BufferMemory(BufferMemoryInput input) : base(input)
    {
    }

    public override OutputValues LoadMemoryVariables(InputValues? inputValues)
    {
        return new OutputValues(new Dictionary<string, object> { { "history", ChatHistory } });
    }
}