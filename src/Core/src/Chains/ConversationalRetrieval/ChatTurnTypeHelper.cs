using System.Text;
using Microsoft.Extensions.AI;

namespace LangChain.Chains.ConversationalRetrieval;

/// <summary>
///
/// </summary>
public static class ChatTurnTypeHelper
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="chatHistory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetChatHistory(IReadOnlyList<ChatMessage> chatHistory)
    {
        chatHistory = chatHistory ?? throw new ArgumentNullException(nameof(chatHistory));

        var buffer = new StringBuilder();

        foreach (var message in chatHistory)
        {
            var rolePrefix = message.Role == ChatRole.User ? "Human: "
                : message.Role == ChatRole.Assistant ? "Assistant: "
                : $"{message.Role}: ";

            buffer.AppendLine($"{rolePrefix}{message.Text}");
        }

        return buffer.ToString();
    }
}
