// ReSharper disable once CheckNamespace

using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiChatSettings : ChatSettings
{
    public new static OpenAiChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        Temperature = 1.0,
        User = string.Empty,
    };
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(User))]
    public string? User { get; init; }

    /// <summary>
    /// Sampling temperature
    /// </summary>
    [MemberNotNull(nameof(Temperature))]
    public double? Temperature { get; init; }
}