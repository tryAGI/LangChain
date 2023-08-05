namespace LangChain.NET.Chat;

public abstract class BaseChatMessage
{
    public BaseChatMessage(string text)
    {
        Text = text;
    }
        
    public string Text { get; set; }
    
    public string? Name { get; set; }

    internal new abstract ChatMessageType GetType();
}