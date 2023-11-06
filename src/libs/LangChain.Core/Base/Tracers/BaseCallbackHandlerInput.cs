namespace LangChain.Base.Tracers;

public class BaseCallbackHandlerInput : IBaseCallbackHandlerInput
{
    public bool IgnoreLlm { get; set; }
    public bool IgnoreRetry { get; set; }
    public bool IgnoreChain { get; set; }
    public bool IgnoreAgent { get; set; }
    public bool IgnoreRetriever { get; set; }
    public bool IgnoreChatModel { get; set; }
}