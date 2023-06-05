namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public enum MessageRole
{
    /// <summary>
    /// Role of message that is a system role.
    /// </summary>
    System,
    
    /// <summary>
    /// Role of message that is spoken by the human.
    /// </summary>
    Human,
    
    /// <summary>
    /// Role of message that is spoken by the AI.
    /// </summary>
    Ai,
    
    /// <summary>
    /// Role of message with arbitrary speaker.
    /// </summary>
    Chat,
}