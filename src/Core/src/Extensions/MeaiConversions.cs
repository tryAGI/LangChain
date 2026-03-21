using LangChain.Providers;
using Microsoft.Extensions.AI;

namespace LangChain.Extensions;

/// <summary>
/// Conversion utilities between LangChain Message types and MEAI ChatMessage types.
/// </summary>
public static class MeaiConversions
{
    /// <summary>
    /// Converts a LangChain MessageRole to an MEAI ChatRole.
    /// </summary>
    public static ChatRole ToChatRole(this MessageRole role)
    {
        return role switch
        {
            MessageRole.System => ChatRole.System,
            MessageRole.Human => ChatRole.User,
            MessageRole.Ai => ChatRole.Assistant,
            MessageRole.ToolCall => ChatRole.Assistant,
            MessageRole.ToolResult => ChatRole.Tool,
            MessageRole.Chat => ChatRole.User,
            _ => ChatRole.User,
        };
    }

    /// <summary>
    /// Converts an MEAI ChatRole to a LangChain MessageRole.
    /// </summary>
    public static MessageRole ToMessageRole(this ChatRole role)
    {
        if (role == ChatRole.System) return MessageRole.System;
        if (role == ChatRole.User) return MessageRole.Human;
        if (role == ChatRole.Assistant) return MessageRole.Ai;
        if (role == ChatRole.Tool) return MessageRole.ToolResult;
        return MessageRole.Human;
    }

    /// <summary>
    /// Converts a LangChain Message to an MEAI ChatMessage.
    /// </summary>
    public static ChatMessage ToChatMessage(this Message message)
    {
        return new ChatMessage(message.Role.ToChatRole(), message.Content);
    }

    /// <summary>
    /// Converts an MEAI ChatMessage to a LangChain Message.
    /// </summary>
    public static Message ToLangChainMessage(this ChatMessage message)
    {
        return new Message(message.Text ?? string.Empty, message.Role.ToMessageRole());
    }

    /// <summary>
    /// Converts a collection of LangChain Messages to MEAI ChatMessages.
    /// </summary>
    public static IList<ChatMessage> ToChatMessages(this IEnumerable<Message> messages)
    {
        return messages.Select(m => m.ToChatMessage()).ToList();
    }

    /// <summary>
    /// Converts a collection of MEAI ChatMessages to LangChain Messages.
    /// </summary>
    public static IList<Message> ToLangChainMessages(this IEnumerable<ChatMessage> messages)
    {
        return messages.Select(m => m.ToLangChainMessage()).ToList();
    }
}
