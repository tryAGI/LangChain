namespace LangChain.Base.Tracers;

/// <summary>
/// Base class for exceptions in tracers module.
/// </summary>
[Serializable]
public class TracerException : Exception
{
    /// <inheritdoc/>
    public TracerException(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public TracerException()
    {
    }

    /// <inheritdoc/>
    public TracerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}