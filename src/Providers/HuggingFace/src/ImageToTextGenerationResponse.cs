using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public class ImageToTextGenerationResponse : List<GeneratedTextItem>;

public sealed class GeneratedTextItem
{
    /// <summary>
    /// The continuated string
    /// </summary>
    [JsonPropertyName("generated_text")]
    public string? GeneratedText { get; set; }
}