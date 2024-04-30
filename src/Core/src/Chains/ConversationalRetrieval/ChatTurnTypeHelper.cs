using System.Text;
using LangChain.Providers;

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
    public static string GetChatHistory(IReadOnlyList<Message> chatHistory)
    {
        chatHistory = chatHistory ?? throw new ArgumentNullException(nameof(chatHistory));

        var buffer = new StringBuilder();

        foreach (var message in chatHistory)
        {
            var rolePrefix = message.Role switch
            {
                MessageRole.Human => "Human: ",
                MessageRole.Ai => "Assistant: ",
                _ => $"{message.Role}: "
            };

            buffer.AppendLine($"{rolePrefix}{message.Content}");
        }

        return buffer.ToString();
    }
}