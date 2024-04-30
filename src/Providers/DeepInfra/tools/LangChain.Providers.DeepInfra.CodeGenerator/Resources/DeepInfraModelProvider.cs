using OpenAI.Constants;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// Contains all the DeepInfra models.
/// </summary>
public static class DeepInfraModelProvider
{
    private static Dictionary<DeepInfraModelIds, ChatModels> Models { get; set; } = new()
    {
        {{DicAdd}}
    };

    [CLSCompliant(false)]
    public static ChatModels GetModelById(DeepInfraModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Deep Infra Model {modelId}");
    }
}