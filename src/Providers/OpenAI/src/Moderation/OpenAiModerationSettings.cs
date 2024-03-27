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
    public new static OpenAiModerationSettings Default { get; } = new();

    /// <summary>
    /// The model to use for moderation.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// The language of the content to be moderated.
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Categories requested for moderation.
    /// </summary>
    public IEnumerable<string>? RequestedCategories { get; init; }
}