using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class ListModelsResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("models")]
    public Model[] Models { get; set; } = [];
}