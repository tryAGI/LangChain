using System.Text;
using LangChain.Providers;

namespace LangChain.Chains.ConversationalRetrieval;

public static class ChatTurnTypeHelper
{
    public static string GetChatHistory(IReadOnlyList<Message> chatHistory)
    {
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