namespace LangChain.Memory;

/// <inheritdoc />
public sealed class BufferMemoryInput : BaseChatMemoryInput
{
    /// <summary>
    /// 
    /// </summary>
    public string AiPrefix { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public string HumanPrefix { get; set; } = string.Empty;
}