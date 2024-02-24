using Amazon;
using Amazon.BedrockRuntime;

namespace LangChain.Providers.Amazon.Bedrock;

/// <summary>
/// 
/// </summary>
public class BedrockProvider(
    RegionEndpoint? region = null)
    : Provider(id: "Bedrock")
{
    #region Properties
    
    [CLSCompliant(false)]
    public AmazonBedrockRuntimeClient Api { get; } = new(region ?? RegionEndpoint.USEast1);
    
    #endregion
}