using System.Text.Json.Serialization;

namespace OllamaTest;

public class GenerateEmbeddingRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("options")]
    public OllamaLanguageModelOptions Options { get; set; }
}

public class GenerateEmbeddingResponse
{
    [JsonPropertyName("embedding")]
    public double[] Embedding { get; set; }
}