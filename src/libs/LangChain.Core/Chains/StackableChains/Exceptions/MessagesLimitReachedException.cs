namespace LangChain.Chains.HelperChains.Exceptions;

public class MessagesLimitReachedException:Exception
{
    public MessagesLimitReachedException(int limit) : base($"Messages limit reached: {limit}")
    {
    }
}