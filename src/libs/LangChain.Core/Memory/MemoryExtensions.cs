using LangChain.Providers;

namespace LangChain.Memory;

public static class MemoryExtensions
{
    public static IReadOnlyCollection<Message> WithHistory(this IReadOnlyCollection<Message> messages, BaseMemory? memory)
    {
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

        return new[]
        {
            history.AsHumanMessage(),
        }.Concat(messages).ToArray();
    }

}