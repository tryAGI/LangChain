using System.Text.Json.Serialization;

namespace LangChain.Providers.Suno.Sdk;

public class GenerateV2Response
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("clips")]
    public IReadOnlyList<Clip> Clips { get; set; } = [];

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; } = new();

    [JsonPropertyName("major_model_version")]
    public string MajorModelVersion { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("batch_size")]
    public int BatchSize { get; set; }
}