namespace LangChain.Chains.HelperChains.Exceptions;

/// <inheritdoc/>
public class StackableChainException : Exception
{
    /// <inheritdoc/>
    public StackableChainException(string message, Exception inner) : base(message, inner)
    {
    }

    /// <inheritdoc/>
    public StackableChainException()
    {
    }

    /// <inheritdoc/>
    public StackableChainException(string message) : base(message)
    {
    }
}