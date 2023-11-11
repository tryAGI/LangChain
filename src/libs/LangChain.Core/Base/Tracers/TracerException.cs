namespace LangChain.Base.Tracers;

/// <summary>
/// Base class for exceptions in tracers module.
/// </summary>
public class TracerException : Exception
{
    public TracerException(string message) : base(message)
    {
    }

    public TracerException()
    {
    }

    public TracerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}