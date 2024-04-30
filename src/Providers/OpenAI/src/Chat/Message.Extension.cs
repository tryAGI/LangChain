using OpenAI;
using System.Text.Json.Nodes;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

internal static class MessageExtensions
{
    private static readonly char[] Separator = [':'];

    public static Message AsFunctionResultMessage(this string json, Tool tool)
    {
        tool = tool ?? throw new ArgumentNullException(nameof(tool));

        return new Message(json, MessageRole.FunctionResult, $"{tool.Function.Name}:{tool.Id}");
    }

    public static IReadOnlyList<Tool> ToToolCalls(this Message message)
    {
        var nameAndId = message.FunctionName?.Split(Separator, StringSplitOptions.RemoveEmptyEntries) ??
                        throw new ArgumentException("Invalid functionCall name and id string");

        if (nameAndId.Length < 2)
            throw new ArgumentException("Invalid functionCall name and id string");

        return [
                new Tool(new Function(nameAndId[0],
                arguments: JsonNode.Parse(message.Content) ?? throw new ArgumentException("Invalid functionCall arguments")))
                {
                    Id = nameAndId[1]
                }
            ];
    }
    public static Tool GetTool(this Message message)
    {
        var nameAndId = message.FunctionName?.Split(Separator, StringSplitOptions.RemoveEmptyEntries) ??
                        throw new ArgumentException("Invalid functionCall name and id string");
        if (nameAndId.Length < 2)
            throw new ArgumentException("Invalid functionCall name and id string");
        return new Tool(new Function(nameAndId[0]))
        {
            Id = nameAndId[1]
        };
    }
}
