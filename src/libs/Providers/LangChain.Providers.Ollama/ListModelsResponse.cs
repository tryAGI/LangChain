using System.Diagnostics;
using System.Text.Json.Serialization;

namespace LangChain.Providers;

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

/// <summary>
/// 
/// </summary>
[DebuggerDisplay("{Name}")]
public class Model
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("modified_at")]
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("digest")]
    public string Digest { get; set; } = string.Empty;
}