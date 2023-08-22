namespace LangChain.Prompts;

public class VariableNode : ParsedFStringNode
{
    public string Name { get; }

    public VariableNode(string name) : base("variable")
    {
        Name = name;
    }
}