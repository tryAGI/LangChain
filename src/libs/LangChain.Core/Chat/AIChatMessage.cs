namespace LangChain.Chat;

/// <inheritdoc />
public sealed class AiChatMessage : BaseChatMessage
{
    /// <inheritdoc />
    public AiChatMessage(string text) : base(text) { }

    internal override ChatMessageType GetType() => ChatMessageType.Ai;
}