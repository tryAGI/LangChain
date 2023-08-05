namespace LangChain.Base;

public abstract class Handler : BaseCallbackHandler
{
    public override IBaseCallbackHandler Copy()
    {
        throw new NotImplementedException();
    }
}