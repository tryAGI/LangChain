namespace LangChain.Chains.HelperChains.Exceptions;

public class MessagesLimitReached:Exception
{
    public MessagesLimitReached(int limit) : base($"Messages limit reached: {limit}")
    {
    }
}