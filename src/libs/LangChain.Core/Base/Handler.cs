namespace LangChain.Base;

/// <inheritdoc />
public abstract class Handler : BaseCallbackHandler
{
    protected Handler(IBaseCallbackHandlerInput input) : base(input)
    {
    }
}