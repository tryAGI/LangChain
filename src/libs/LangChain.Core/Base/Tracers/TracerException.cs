namespace LangChain.Base.Tracers;

/// <summary>
/// Base class for exceptions in tracers module.
/// </summary>
public class TracerException : Exception
{
    public TracerException(string message) : base(message)
    {
    }
}