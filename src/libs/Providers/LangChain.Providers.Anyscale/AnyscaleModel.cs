using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Anyscale;

/// <summary>
/// 
/// </summary>
public class AnyscaleModel(
    AnyscaleProvider provider,
    string id)
    : OpenAiChatModel(provider, id);