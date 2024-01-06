using System.Diagnostics;
using System.Text.Json.Serialization;

namespace OllamaTest;

public class ListModelsResponse
{
    [JsonPropertyName("models")]
    public Model[] Models { get; set; }
}

[DebuggerDisplay("{Name}")]
public class Model
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("modified_at")]
    public DateTime ModifiedAt { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("digest")]
    public string Digest { get; set; }
}