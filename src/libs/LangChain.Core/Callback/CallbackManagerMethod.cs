namespace LangChain.Callback;

/// <summary>
/// 
/// </summary>
public delegate Task<object> CallbackManagerMethod(params object[] args);