namespace LangChain.Chains.HelperChains.Exceptions;

public class StackableChainException : Exception
{
    public StackableChainException(string message, Exception inner) : base(message, inner)
    {
    }

    public StackableChainException()
    {
    }
}