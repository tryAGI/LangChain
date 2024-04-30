// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image to text request settings.
/// </summary>
public class ImageToTextSettings
{
    public static ImageToTextSettings Default { get; } = new()
    {
        User = string.Empty,
    };

    /// <summary>
    /// Unique user identifier.
    /// </summary>
    public string? User { get; init; }


    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static ImageToTextSettings Calculate(
        ImageToTextSettings? requestSettings,
        ImageToTextSettings? modelSettings,
        ImageToTextSettings? providerSettings)
    {
        return new ImageToTextSettings
        {
            User =
                requestSettings?.User ??
                modelSettings?.User ??
                providerSettings?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
        };
    }
}