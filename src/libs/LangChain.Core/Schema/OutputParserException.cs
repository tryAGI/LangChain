namespace LangChain.Schema;

public class OutputParserException : Exception
{
    public string Output { get; }

    public OutputParserException(string message, string? output = null) : base(message)
    {
        Output = output;
    }

    public OutputParserException()
    {
    }

    public OutputParserException(string message) : base(message)
    {
    }

    public OutputParserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}