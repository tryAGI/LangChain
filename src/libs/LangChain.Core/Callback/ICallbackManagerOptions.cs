namespace LangChain.NET.Callback;

public interface ICallbackManagerOptions
{
    bool Verbose { get; set; }
    bool Tracing { get; set; }
}