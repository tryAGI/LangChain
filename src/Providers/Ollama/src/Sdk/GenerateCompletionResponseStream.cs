using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class GenerateCompletionResponseStream
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; set; }
}