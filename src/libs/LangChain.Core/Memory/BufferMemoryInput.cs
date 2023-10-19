namespace LangChain.Memory;

public sealed class BufferMemoryInput : BaseChatMemoryInput
{
    public string AiPrefix { get; set; }
    public string HumanPrefix { get; set; }
}