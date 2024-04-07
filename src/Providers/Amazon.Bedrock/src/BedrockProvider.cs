using Amazon;
using Amazon.BedrockRuntime;

namespace LangChain.Providers.Amazon.Bedrock;

/// <summary>
/// Represents a provider for Amazon Bedrock.
/// </summary>
public class BedrockProvider : Provider
{
    private const string DefaultProviderId = "Bedrock";

    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockProvider"/> class with the default region.
    /// </summary>
    public BedrockProvider() : this(RegionEndpoint.USEast1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockProvider"/> class with the specified region.
    /// </summary>
    /// <param name="region">The region endpoint for the Amazon Bedrock service.</param>
    public BedrockProvider(RegionEndpoint region) : base(DefaultProviderId)
    {
        Api = new AmazonBedrockRuntimeClient(region);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockProvider"/> class with the specified access key, secret key, and region.
    /// </summary>
    /// <param name="accessKeyId">The access key ID for the Amazon Bedrock service.</param>
    /// <param name="secretAccessKey">The secret access key for the Amazon Bedrock service.</param>
    /// <param name="region">The region endpoint for the Amazon Bedrock service. Defaults to US East 1.</param>
    public BedrockProvider(string accessKeyId, string secretAccessKey, RegionEndpoint? region = null)
        : base(DefaultProviderId)
    {
        Api = new AmazonBedrockRuntimeClient(accessKeyId, secretAccessKey, region ?? RegionEndpoint.USEast1);
    }

    #region Properties

    [CLSCompliant(false)]
    public AmazonBedrockRuntimeClient Api { get; }

    #endregion
}