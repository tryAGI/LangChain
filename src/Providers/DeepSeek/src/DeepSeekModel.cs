using LangChain.Providers.OpenAI;

namespace LangChain.Providers.DeepSeek;

/// <summary>
/// 
/// </summary>
public class DeepSeekModel(
    DeepSeekProvider provider,
    string id)
    : OpenAiChatModel(provider, DeepSeekModels.GetModelById(id));