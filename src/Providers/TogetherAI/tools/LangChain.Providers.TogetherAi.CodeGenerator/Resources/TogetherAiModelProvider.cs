using OpenAI;

namespace LangChain.Providers.TogetherAi;

/// <summary>
/// Contains all the Together Ai models.
/// </summary>
public static class TogetherAiModelProvider
{
    private static Dictionary<TogetherAiModelIds, ChatModelMetadata> Models { get; set; } = new()
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
    public static ChatModelMetadata GetModelById(TogetherAiModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Together Ai Model {modelId}");
    }
}