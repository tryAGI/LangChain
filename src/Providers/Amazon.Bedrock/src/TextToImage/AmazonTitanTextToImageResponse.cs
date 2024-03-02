using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonTitanTextToImageResponse
{
    [JsonPropertyName("images")]
    public IReadOnlyList<string> Images { get; set; } = new List<string>();
}