namespace LangChain.Chat;

internal sealed class SystemChatMessage : BaseChatMessage
{
    public SystemChatMessage(string text) : base(text) { }

    internal override ChatMessageType GetType() => ChatMessageType.System;
}