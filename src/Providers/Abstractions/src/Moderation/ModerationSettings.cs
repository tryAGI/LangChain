// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

/// <summary>
/// Base class for moderation request settings.
/// </summary>
public class ModerationSettings
{
    public static ModerationSettings Default { get; } = new();
}