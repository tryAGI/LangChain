using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class GenerateEmbeddingRequest
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("options")]
    public OllamaOptions Options { get; set; } = new();
}