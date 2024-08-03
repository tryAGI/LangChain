using OpenAI;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// Contains all the DeepInfra models.
/// </summary>
public static class DeepInfraModelProvider
{
    private static Dictionary<DeepInfraModelIds, ChatModelMetadata> Models { get; set; } = new()
    {
        {{DicAdd}}
    };

    public static ChatModelMetadata ToMetadata(string? id, int? contextLength, double? pricePerInputTokenInUsd, double? pricePerOutputTokenInUsd)
    {
        return new ChatModelMetadata
        {
            Id = id,
            ContextLength = contextLength,
            PricePerInputTokenInUsd = pricePerInputTokenInUsd,
            PricePerOutputTokenInUsd = pricePerOutputTokenInUsd,
        };
    }

    [CLSCompliant(false)]
    public static ChatModelMetadata GetModelById(DeepInfraModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Deep Infra Model {modelId}");
    }
}