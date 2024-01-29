using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for chat request settings.
/// </summary>
public class ChatSettings
{
    public static ChatSettings Default { get; } = new()
    {
        StopSequences = Array.Empty<string>(),
    };
    
    /// <summary>
    /// Defines the stop sequences for the model.
    /// </summary>
    [MemberNotNull(nameof(StopSequences))]
    public IReadOnlyList<string>? StopSequences { get; init; }
}