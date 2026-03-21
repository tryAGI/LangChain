#pragma warning disable CA1052

namespace LangChain.Providers;

/// <summary>
/// Text-to-image settings.
/// </summary>
public class TextToImageSettings
{
    public static TextToImageSettings Default { get; } = new();
}
