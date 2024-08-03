using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Fireworks;

/// <summary>
/// 
/// </summary>
public class FireworksConfiguration : OpenAiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public new const string SectionName = "Fireworks";

    /// <summary>
    /// 
    /// </summary>
    public new string? Endpoint { get; set; } = "https://api.fireworks.ai/inference/v1";
}