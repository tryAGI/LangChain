namespace LangChain.Memory;

/// <summary>
/// 
/// </summary>
public class BaseChatMemoryInput
{
    /// <summary>
    /// 
    /// </summary>
    public BaseChatMessageHistory ChatHistory { get; set; } = new ChatMessageHistory();
    
    /// <summary>
    /// 
    /// </summary>
    public string InputKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public string MemoryKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public bool ReturnMessages { get; set; }
}