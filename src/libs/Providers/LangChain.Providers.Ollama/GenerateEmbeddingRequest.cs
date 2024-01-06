using System.Text.Json.Serialization;

namespace LangChain.Providers;

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
    public OllamaLanguageModelOptions Options { get; set; } = new();
}

/// <summary>
/// 
/// </summary>
public class GenerateEmbeddingResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("embedding")]
    public double[] Embedding { get; set; } = [];
}