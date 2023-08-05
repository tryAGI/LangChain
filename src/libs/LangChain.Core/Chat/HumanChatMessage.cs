namespace LangChain.Chat;

public sealed class HumanChatMessage : BaseChatMessage
{
    public HumanChatMessage(string text) : base(text){}
    
    internal override ChatMessageType GetType() => ChatMessageType.User;
}