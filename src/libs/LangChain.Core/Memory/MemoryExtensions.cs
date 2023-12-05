using LangChain.Providers;

namespace LangChain.Memory;

/// <summary>
/// 
/// </summary>
public static class MemoryExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="memory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IReadOnlyCollection<Message> WithHistory(
        this IReadOnlyCollection<Message> messages,
        BaseMemory? memory)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));
        
        if (memory == null)
        {
            return messages;
        }

        var history = "These are our previous conversations:\n";
        var previousMessages = memory.LoadMemoryVariables(null);
        if (previousMessages.Value is { } messageDict &&
            messageDict["history"] is ChatMessageHistory msg)
        {
            foreach (var chatMessage in msg.Messages)
            {
                history += chatMessage.Content + "\n";
            }
        }

        var result = new Message[messages.Count + 1];
        result[0] = history.AsHumanMessage();
        messages.CopyTo(result, startIndex: 1);

        return result;
    }

    private static void CopyTo<T>(this IReadOnlyCollection<T> source, T[] destination, int startIndex)
    {
        if (destination.Length > source.Count + startIndex)
        {
            throw new ArgumentException(
                $"{nameof(destination)} required to have min length of {source.Count + startIndex}, but was {destination.Length}");
        }

        var i = 0;
        foreach (var item in source)
        {
            destination[startIndex + i] = item;
            i++;
        }
    }
}