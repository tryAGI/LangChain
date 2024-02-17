using Amazon;

namespace LangChain.Providers.Amazon.Bedrock.Embeddings;

public class BedrockEmbeddingsConfiguration
{
    public string ModelId { get; set; }
    public RegionEndpoint Region { get; set; } = RegionEndpoint.USEast1;
    public string Base64Image { get; set; }
}