// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

/// <summary>
/// Base class for speech-to-text request settings.
/// </summary>
public class SpeechToTextSettings
{
    public static SpeechToTextSettings Default { get; } = new();
}