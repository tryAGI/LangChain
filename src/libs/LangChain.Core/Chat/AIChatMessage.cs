namespace LangChain.NET.Chat;

internal sealed class AiChatMessage : BaseChatMessage
{
    public AiChatMessage(string text) : base(text){}
    
    internal override ChatMessageType GetType() => ChatMessageType.Ai;
}