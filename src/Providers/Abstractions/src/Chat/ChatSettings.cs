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
        User = string.Empty,
        UseStreaming = false,
    };

    /// <summary>
    /// Unique user identifier.
    /// </summary>
    public string? User { get; init; }

    /// <summary>
    /// Defines the stop sequences for the model.
    /// Up to 4 sequences where the API will stop generating further tokens. <br/>
    /// </summary>
    public IReadOnlyList<string>? StopSequences { get; init; }

    /// <summary>
    /// If set, partial message deltas will be sent, like in ChatGPT. <br/>
    /// Tokens will be sent as data-only server-sent events as they become available. <br/>
    /// Enabling disables tokenUsage reporting <br/>
    /// Defaults to false. <br/>
    /// </summary>
    public bool? UseStreaming { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static ChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        return new ChatSettings
        {
            StopSequences =
                requestSettings?.StopSequences ??
                modelSettings?.StopSequences ??
                providerSettings?.StopSequences ??
                Default.StopSequences ??
                throw new InvalidOperationException("Default StopSequences is not set."),
            User =
                requestSettings?.User ??
                modelSettings?.User ??
                providerSettings?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
            UseStreaming =
                requestSettings?.UseStreaming ??
                modelSettings?.UseStreaming ??
                providerSettings?.UseStreaming ??
                Default.UseStreaming ??
                throw new InvalidOperationException("Default UseStreaming is not set."),
        };
    }
}