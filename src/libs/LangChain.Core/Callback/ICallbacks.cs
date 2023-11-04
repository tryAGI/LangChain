using LangChain.Base;

namespace LangChain.Callback;

public interface ICallbacks;

public record ManagerCallbacks(CallbackManager Value) : ICallbacks;

public record HandlersCallbacks(List<BaseCallbackHandler> Value) : ICallbacks;