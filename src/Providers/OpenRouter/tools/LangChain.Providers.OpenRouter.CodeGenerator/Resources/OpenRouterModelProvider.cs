using OpenAI;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// Contains all the OpenRouter models.
/// </summary>
public static class OpenRouterModelProvider
{
    private static Dictionary<OpenRouterModelIds, ChatModelMetadata> Models { get; set; } = new()
    {
        {{DicAdd}}
    };

    [CLSCompliant(false)]
    public static ChatModelMetadata GetModelById(OpenRouterModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Open Router Model {modelId}");
    }

    [CLSCompliant(false)]
    public static ChatModelMetadata GetModelById(string modelId)
    {
        return Models.Values.First(s => s.Id == modelId);
    }
}