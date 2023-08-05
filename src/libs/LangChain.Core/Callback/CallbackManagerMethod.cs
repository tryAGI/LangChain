namespace LangChain.Callback;

public delegate Task<object> CallbackManagerMethod(params object[] args);