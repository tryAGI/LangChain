using LangChain.Providers.OpenAI;
using OpenAI.Constants;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// </summary>
public class DeepInfraModel(DeepInfraProvider provider, ChatModels model) : OpenAiChatModel(provider, model)
{
    public DeepInfraModel(DeepInfraProvider provider,
        DeepInfraModelIds id) : this(provider, DeepInfraModelProvider.GetModelById(id))
    {
    }

    public DeepInfraModel(DeepInfraProvider provider, string id) : this(provider, new ChatModels(
        id,
        0,
        PricePerOutputTokenInUsd: 0.0,
        PricePerInputTokenInUsd: 0.0))
    {
    }

   
}