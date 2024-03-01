using System.Text.Json.Serialization;

namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonTitanTextToImageResponse
{
    [JsonPropertyName("images")]
    public IList<string> Images { get; set; }
}