namespace LangChain.Base.Tracers;

/// <inheritdoc />
public class BaseCallbackHandlerInput : IBaseCallbackHandlerInput
{
    /// <inheritdoc />
    public bool IgnoreLlm { get; set; }

    /// <inheritdoc />
    public bool IgnoreRetry { get; set; }

    /// <inheritdoc />
    public bool IgnoreChain { get; set; }

    /// <inheritdoc />
    public bool IgnoreAgent { get; set; }

    /// <inheritdoc />
    public bool IgnoreRetriever { get; set; }

    /// <inheritdoc />
    public bool IgnoreChatModel { get; set; }
}