using System.Text.Json.Nodes;

namespace LangChain.Providers.Anthropic.Tools;

public class AnthropicToolCall
{
    public string FunctionName { get; set; } = string.Empty;
    public JsonNode? Arguments { get; set; }
}