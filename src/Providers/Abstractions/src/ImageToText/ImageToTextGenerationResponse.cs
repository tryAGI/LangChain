using System.Text.Json.Serialization;

namespace LangChain.Providers;

public  class ImageToTextGenerationResponse : List<ImageToTextGenerationResponse.GeneratedTextItem>
{
    public sealed class GeneratedTextItem
    {
        /// <summary>
        /// The continuated string
        /// </summary>
        [JsonPropertyName("generated_text")]
        public string? GeneratedText { get; set; }
    }
}