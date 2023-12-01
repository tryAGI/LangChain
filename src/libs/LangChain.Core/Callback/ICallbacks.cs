using LangChain.Base;

namespace LangChain.Callback;

public interface ICallbacks;

public static class ManagerCallbacksExtensions
{
    public static ManagerCallbacks ToCallbacks(this ParentRunManager source) => new ManagerCallbacks(source.GetChild());
}

public record ManagerCallbacks(CallbackManager Value) : ICallbacks;

public record HandlersCallbacks(List<BaseCallbackHandler> Value) : ICallbacks;