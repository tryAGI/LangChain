using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// Contains all the OpenRouter models.
/// </summary>
public static class OpenRouterModelProvider
{
    private static Dictionary<OpenRouterModelIds, ChatModels> Models { get; set; } = new()
    {
        {{DicAdd}}
    };

    [CLSCompliant(false)]
    public static ChatModels GetModelById(OpenRouterModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Open Router Model {modelId}");
    }

    [CLSCompliant(false)]
    public static ChatModels GetModelById(string modelId)
    {
        var model = Models.Values.FirstOrDefault(s => s.Id == modelId);
        if (model == null)
            throw new KeyNotFoundException($"Model with ID {modelId} not found.");
        return model;
    }
}