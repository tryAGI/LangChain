// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

/// <summary>
/// Base class for text-to-speech request settings.
/// </summary>
public class TextToSpeechSettings
{
    public static TextToSpeechSettings Default { get; } = new();
}