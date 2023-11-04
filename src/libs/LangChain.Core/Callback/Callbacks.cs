using LangChain.Base;

namespace LangChain.Callback;

public interface Callbacks;
public record ManagerCallbacks(CallbackManager Value) : Callbacks;

public record HandlersCallbacks(List<BaseCallbackHandler> Value) : Callbacks;