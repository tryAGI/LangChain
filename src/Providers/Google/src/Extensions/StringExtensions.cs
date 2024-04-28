using System.Text.Json;
using System.Text.Json.Nodes;
using GenerativeAI.Extensions;
using GenerativeAI.Tools;
using GenerativeAI.Types;

namespace LangChain.Providers.Google.Extensions;

/// <summary>
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     To Model/Assistant Content
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static Content AsModelContent(this string message)
    {
        var content = new Content([
            new Part
            {
                Text = message
            }
        ], Roles.Model);
        return content;
    }

    /// <summary>
    ///     To Model/Assistant Content
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static Content AsUserContent(this string message)
    {
        var content = new Content([
            new Part
            {
                Text = message
            }
        ], Roles.User);
        return content;
    }

    /// <summary>
    ///     To Model/Assistant Content
    /// </summary>
    /// <param name="args">Serialized Arguments</param>
    /// <param name="functionName">Function name</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static Content AsFunctionCallContent(this string args, string functionName)
    {
        var content = new Content([
            new Part
            {
                FunctionCall = new ChatFunctionCall
                {
                    Arguments = JsonSerializer.Deserialize(args, SourceGenerationContext.Default.DictionaryStringObject),
                    Name = functionName
                }
            }
        ], Roles.Model);
        return content;
    }

    /// <summary>
    ///     To Model/Assistant Content
    /// </summary>
    /// <param name="args">Serialized Arguments</param>
    /// <param name="functionName">Function name</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static Content AsFunctionResultContent(this string args, string functionName)
    {
        return JsonNode.Parse(args).ToFunctionCallContent(functionName);
    }
}