using LangChain.Providers.OpenAI;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// </summary>
public class DeepInfraModel(DeepInfraProvider provider, string id) : OpenAiChatModel(provider, id)
{
    public DeepInfraModel(DeepInfraProvider provider,
        DeepInfraModelIds id) : this(provider, DeepInfraModelProvider.GetModelById(id).Id ?? throw new InvalidOperationException("Model not found"))
    {
    }
}