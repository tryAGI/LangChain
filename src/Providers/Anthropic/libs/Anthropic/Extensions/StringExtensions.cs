using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Anthropic.SDK.Messaging;
using LangChain.Providers.Anthropic.Helpers;
using LangChain.Providers.Anthropic.Tools;

namespace LangChain.Providers.Anthropic.Extensions;

/// <summary>
/// </summary>
public static class StringExtensions
{
    private static JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static global::Anthropic.SDK.Messaging.Message AsHumanMessage(this string content)
    {
        return new global::Anthropic.SDK.Messaging.Message { Content = content, Role = RoleType.User };
    }

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static global::Anthropic.SDK.Messaging.Message AsAssistantMessage(this string content)
    {
        return new global::Anthropic.SDK.Messaging.Message { Content = content, Role = RoleType.Assistant };
    }

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string AsPrompt(this string content)
    {
        return $"\n\n{content.AsHumanMessage()}\n\nAssistant:";
    }

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string AsPrompt(this string[] content)
    {
        return AsPrompt(string.Join("\n\n", content));
    }

    public static AnthropicToolCall ToAnthropicToolCall(this string content, IEnumerable<AnthropicTool> tools)
    {
        var toolCallXml = TextHelper.GetTextBetweenDelimiters(content, "<function_calls>", "</function_calls>");
        var doc = new XmlDocument();
        doc.LoadXml(toolCallXml);

        var toolCall = new AnthropicToolCall();
        var toolNames = doc.GetElementsByTagName("tool_name");
        if (toolNames.Count > 0)
        {
            var name = toolNames[0];
            toolCall.FunctionName = name.InnerText;
        }

        var parameters = doc.GetElementsByTagName("parameters");
        var tool = tools.FirstOrDefault(s => s.Name == toolCall.FunctionName);
        if (parameters.Count > 0)
        {
            var @params = new Dictionary<string, object>();
            foreach (XmlNode node in parameters[0].ChildNodes)
                @params.Add(node.Name, ToolCallParamParser.ParseData(node.Name, tool, node.InnerText));

            toolCall.Arguments = JsonSerializer.Serialize(@params, SerializerOptions);
        }

        return toolCall;
    }

    public static Message ToAnthropicToolResponseMessage(this string message, string functionName)
    {
        var res =
            $"<function_results>\r\n<result>\r\n<tool_name>{functionName}</tool_name>\r\n<stdout>\r\n{message}\r\n</stdout>\r\n</result>\r\n</function_results>";

        return new Message(res, MessageRole.Human);
    }
}