// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Stability;

/// <inheritdoc />
public class StableDiffusionExtraLargeV1Model(BedrockProvider provider)
    : StableDiffusionTextToImageModel(provider, id: "stability.stable-diffusion-xl-v1");