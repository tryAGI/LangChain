using LangChain.Base;

namespace LangChain.Callback;

public interface ICallbacks;

public static class ManagerCallbacksExtensions
{
    public static ManagerCallbacks ToCallbacks(this ParentRunManager source)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        
        return new ManagerCallbacks(source.GetChild());
    }
}

public record ManagerCallbacks(CallbackManager Value) : ICallbacks;

public record HandlersCallbacks(List<BaseCallbackHandler> Value) : ICallbacks;