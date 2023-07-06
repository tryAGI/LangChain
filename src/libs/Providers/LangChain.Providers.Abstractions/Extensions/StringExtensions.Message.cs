using System.Text;

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public static class MessageStringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsSystemMessage(this string text)
    {
        return new Message(text, MessageRole.System);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsHumanMessage(this string text)
    {
        return new Message(text, MessageRole.Human);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsAiMessage(this string text)
    {
        return new Message(text, MessageRole.Ai);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsChatMessage(this string text)
    {
        return new Message(text, MessageRole.Chat);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    public static Message AsFunctionCallMessage(this string text, string functionName)
    {
        return new Message(text, MessageRole.FunctionCall, FunctionName: functionName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    public static Message AsFunctionResultMessage(this string text, string functionName)
    {
        return new Message(text, MessageRole.FunctionResult, FunctionName: functionName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public static string AsHistory(this IReadOnlyCollection<Message> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));
        
        var builder = new StringBuilder(capacity: messages.Count * 64);
        foreach(var message in messages)
        {
            builder.Append(message.Role switch
            {
                MessageRole.System => "System: ",
                MessageRole.Ai => "AI: ",
                MessageRole.FunctionCall => "Function call: ",
                MessageRole.Human => "Human: ",
                MessageRole.FunctionResult => "Function result: ",
                _ => "Human: ",
            });
            if (message.Role is MessageRole.FunctionCall or MessageRole.FunctionResult)
            {
                builder.Append(message.FunctionName);
            }
            if (message.Role == MessageRole.FunctionCall)
            {
                builder.Append('(');
            }
            else if (message.Role == MessageRole.FunctionResult)
            {
                builder.Append(" -> ");
            }
            builder.Append(message.Content);
            if (message.Role == MessageRole.FunctionCall)
            {
                builder.Append(')');
            }
            builder.AppendLine();
        }

        return builder.ToString();
    }
}