using Amazon;

namespace LangChain.Providers.Bedrock.Models;

public class BedrockConfiguration
{
    public string? ModelId { get; set; }
    public RegionEndpoint Region { get; set; } = RegionEndpoint.USEast1;
    public double Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 4096;
}