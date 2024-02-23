using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class GenerateCompletionDoneResponseStream : GenerateCompletionResponseStream
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("context")]
    public long[] Context { get; set; } = Array.Empty<long>();

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("load_duration")]
    public long LoadDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("prompt_eval_duration")]
    public long PromptEvalDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("eval_count")]
    public int EvalCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("eval_duration")]
    public long EvalDuration { get; set; }
}