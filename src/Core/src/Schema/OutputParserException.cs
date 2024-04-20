namespace LangChain.Schema;

/// <inheritdoc/>
[Serializable]
public class OutputParserException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public string Output { get; } = string.Empty;

    /// <inheritdoc/>
    public OutputParserException(string message, string? output = null) : base(message)
    {
        Output = output ?? string.Empty;
    }

    /// <inheritdoc/>
    public OutputParserException()
    {
    }

    /// <inheritdoc/>
    public OutputParserException(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public OutputParserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}