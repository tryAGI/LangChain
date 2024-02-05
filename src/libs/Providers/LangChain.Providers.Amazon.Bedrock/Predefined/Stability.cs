using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Stability;

/// <inheritdoc />
public class StableDiffusionExtraLargeV0Model(RegionEndpoint? region = null)
    : StableDiffusionImageGenerationModel(new BedrockProvider(region), id: "stability.stable-diffusion-xl-v0");

/// <inheritdoc />
public class StableDiffusionExtraLargeV1Model(RegionEndpoint? region = null)
    : StableDiffusionImageGenerationModel(new BedrockProvider(region), id: "stability.stable-diffusion-xl-v1");