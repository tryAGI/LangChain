namespace LangChain.NET.Chat;

internal class ChatMessage : BaseChatMessage
{
    public ChatMessage(string text) : base(text){}

    
    internal override ChatMessageType GetType() => ChatMessageType.Generic;
}