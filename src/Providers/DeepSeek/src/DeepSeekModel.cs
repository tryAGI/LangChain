using LangChain.Providers.OpenAI;

namespace LangChain.Providers.DeepSeek;

/// <summary>
/// </summary>
public class DeepSeekModel(
    DeepSeekProvider provider,
    string id)
    : OpenAiChatModel(provider, id)
{
    /// <inheritdoc/>
    public override int ContextLength => 16_000;
}