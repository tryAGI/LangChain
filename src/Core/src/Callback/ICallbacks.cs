using LangChain.Base;

namespace LangChain.Callback;

/// <summary>
/// 
/// </summary>
public interface ICallbacks;

/// <summary>
/// 
/// </summary>
public static class ManagerCallbacksExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ManagerCallbacks ToCallbacks(this ParentRunManager source)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));

        return new ManagerCallbacks(source.GetChild());
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="Value"></param>
public record ManagerCallbacks(CallbackManager Value) : ICallbacks;

/// <summary>
/// 
/// </summary>
/// <param name="Value"></param>
public record HandlersCallbacks(List<BaseCallbackHandler> Value) : ICallbacks;