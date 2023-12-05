namespace LangChain.Memory;

public class BaseChatMemoryInput
{
    public BaseChatMessageHistory ChatHistory { get; set; } = new ChatMessageHistory();
    public string InputKey { get; set; } = string.Empty;
    public string MemoryKey { get; set; } = string.Empty;
    public bool ReturnMessages { get; set; }
}