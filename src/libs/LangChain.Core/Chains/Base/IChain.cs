using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Abstractions.Chains.Base;

/// <summary>
/// 
/// </summary>
public interface IChain
{
    /// <summary>
    /// 
    /// </summary>
    IReadOnlyList<string> InputKeys { get; }
    
    /// <summary>
    /// 
    /// </summary>
    IReadOnlyList<string> OutputKeys { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<string?> Run(string input);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="callbacks"></param>
    /// <returns></returns>
    Task<string> Run(Dictionary<string, object> input, ICallbacks? callbacks = null);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="callbacks"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    Task<IChainValues> CallAsync(IChainValues values,
        ICallbacks? callbacks = null,
        IReadOnlyList<string>? tags = null,
        IReadOnlyDictionary<string, object>? metadata = null);
}