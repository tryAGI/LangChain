namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public interface IBaseCallbackHandlerInput
{
    /// <summary> Whether to ignore LLM callbacks. </summary>
    bool IgnoreLlm { get; set; }

    /// <summary> Whether to ignore retry callbacks. </summary>
    bool IgnoreRetry { get; set; }

    /// <summary> Whether to ignore chain callbacks. </summary>
    bool IgnoreChain { get; set; }

    /// <summary> Whether to ignore agent callbacks. </summary>
    bool IgnoreAgent { get; set; }

    /// <summary> Whether to ignore retriever callbacks. </summary>
    bool IgnoreRetriever { get; set; }

    /// <summary> Whether to ignore chat model callbacks. </summary>
    bool IgnoreChatModel { get; set; }
}