namespace LangChain.Chat;

public sealed class AiChatMessage : BaseChatMessage
{
    public AiChatMessage(string text) : base(text){}
    
    internal override ChatMessageType GetType() => ChatMessageType.Ai;
}