using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Fireworks;

/// <summary>
/// 
/// </summary>
public class FireworksModel(
    FireworksProvider provider,
    string id)
    : OpenAiChatModel(provider, id);