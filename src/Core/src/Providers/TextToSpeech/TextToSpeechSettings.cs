#pragma warning disable CA1052

namespace LangChain.Providers;

/// <summary>
/// Text-to-speech settings.
/// </summary>
public class TextToSpeechSettings
{
    public static TextToSpeechSettings Default { get; } = new();
}
