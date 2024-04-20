namespace LangChain.Base;

/// <inheritdoc />
public abstract class Handler : BaseCallbackHandler
{
    /// <inheritdoc />
    protected Handler(IBaseCallbackHandlerInput input) : base(input)
    {
    }
}