namespace LangChain.Prompts;

/// <inheritdoc/>
public class LiteralNode(
    string text)
    : ParsedFStringNode("literal")
{
    /// <summary>
    /// 
    /// </summary>
    public string Text { get; } = text;
}