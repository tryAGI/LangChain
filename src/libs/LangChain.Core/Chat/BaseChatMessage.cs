namespace LangChain.Chat;

/// <summary>
/// 
/// </summary>
public abstract class BaseChatMessage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    public BaseChatMessage(string text)
    {
        Text = text;
    }
        
    /// <summary>
    /// 
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Name { get; set; }

    internal new abstract ChatMessageType GetType();
}