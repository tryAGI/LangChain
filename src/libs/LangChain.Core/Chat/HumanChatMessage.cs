namespace LangChain.Chat;

/// <inheritdoc />
public sealed class HumanChatMessage : BaseChatMessage
{
    /// <inheritdoc />
    public HumanChatMessage(string text) : base(text){}
    
    internal override ChatMessageType GetType() => ChatMessageType.User;
}