using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OpenAiModerationSettings : ModerationSettings
{
    /// <summary>
    /// 
    /// </summary>
    public new static OpenAiModerationSettings Default { get; } = new()
    {
        User = string.Empty,
    };

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(User))]
    public string? User { get; init; }
}