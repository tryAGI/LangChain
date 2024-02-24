using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

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