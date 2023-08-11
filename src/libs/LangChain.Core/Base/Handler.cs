namespace LangChain.Base;

/// <inheritdoc />
public abstract class Handler : BaseCallbackHandler
{
    /// <inheritdoc />
    public override IBaseCallbackHandler Copy()
    {
        throw new NotImplementedException();
    }
}