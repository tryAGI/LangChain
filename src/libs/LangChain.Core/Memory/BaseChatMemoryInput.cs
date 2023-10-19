namespace LangChain.Memory;

public class BaseChatMemoryInput
{
    public BaseChatMessageHistory ChatHistory { get; set; }
    public string InputKey { get; set; }
    public string MemoryKey { get; set; }
    public bool ReturnMessages { get; set; }
}