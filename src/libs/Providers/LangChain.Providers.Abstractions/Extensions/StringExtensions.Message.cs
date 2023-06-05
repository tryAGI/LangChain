namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public static class MessageStringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsSystemMessage(this string text)
    {
        return new Message(text, MessageRole.System);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsHumanMessage(this string text)
    {
        return new Message(text, MessageRole.Human);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsAiMessage(this string text)
    {
        return new Message(text, MessageRole.Ai);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsChatMessage(this string text)
    {
        return new Message(text, MessageRole.Chat);
    }
}