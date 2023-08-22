namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public interface IBaseCallbackHandlerInput
{
    /// <summary>
    /// 
    /// </summary>
    bool IgnoreLlm { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool IgnoreChain { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool IgnoreAgent { get; set; }
}