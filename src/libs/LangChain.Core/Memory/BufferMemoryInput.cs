namespace LangChain.Memory;

public sealed class BufferMemoryInput : BaseChatMemoryInput
{
    public string AiPrefix { get; set; } = string.Empty;
    public string HumanPrefix { get; set; } = string.Empty;
}