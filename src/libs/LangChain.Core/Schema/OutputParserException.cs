namespace LangChain.NET.Schema;

public class OutputParserException : Exception
{
    public string Output { get; }

    public OutputParserException(string message, string? output = null) : base(message)
    {
        Output = output;
    }
}