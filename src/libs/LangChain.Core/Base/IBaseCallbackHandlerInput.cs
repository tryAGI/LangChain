namespace LangChain.NET.Base;

public interface IBaseCallbackHandlerInput
{
    bool IgnoreLlm { get; set; }
    bool IgnoreChain { get; set; }
    bool IgnoreAgent { get; set; }
}