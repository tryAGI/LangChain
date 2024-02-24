namespace LangChain.Callback;

/// <summary>
/// 
/// </summary>
public interface ICallbackManagerOptions
{
    /// <summary>
    /// 
    /// </summary>
    bool Verbose { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    bool Tracing { get; set; }
}