// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

/// <summary>
/// Base class for image generation request settings.
/// </summary>
public class TextToMusicSettings
{
    public static TextToMusicSettings Default { get; } = new();
}