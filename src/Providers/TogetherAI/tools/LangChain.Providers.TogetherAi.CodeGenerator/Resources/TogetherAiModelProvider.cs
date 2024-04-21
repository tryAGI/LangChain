using OpenAI.Constants;

namespace LangChain.Providers.TogetherAi;

/// <summary>
/// Contains all the Together Ai models.
/// </summary>
public static class TogetherAiModelProvider
{
    private static Dictionary<TogetherAiModelIds, ChatModels> Models { get; set; } = new()
    {
        {{DicAdd}}
    };

    public static ChatModels GetModelById(TogetherAiModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Together Ai Model {modelId}");
    }
}