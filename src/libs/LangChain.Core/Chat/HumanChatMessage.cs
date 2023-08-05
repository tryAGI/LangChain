namespace LangChain.NET.Chat;

internal sealed class HumanChatMessage : BaseChatMessage
{
    public HumanChatMessage(string text) : base(text){}
    
    internal override ChatMessageType GetType() => ChatMessageType.User;
}