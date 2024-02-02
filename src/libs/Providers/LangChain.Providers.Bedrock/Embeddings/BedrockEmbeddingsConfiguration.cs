using Amazon;

namespace LangChain.Providers.Bedrock.Embeddings;

public class BedrockEmbeddingsConfiguration
{
    public string ModelId { get; set; }
    public RegionEndpoint Region { get; set; } = RegionEndpoint.USEast1;
}