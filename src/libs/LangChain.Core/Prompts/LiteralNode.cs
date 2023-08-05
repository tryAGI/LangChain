namespace LangChain.Prompts;

public class LiteralNode : ParsedFStringNode
{
    public string Text { get; }
    
    public LiteralNode(string text) : base("literal")
    {
        Text = text;
    }
}