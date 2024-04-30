// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for embedding requests.
/// </summary>
public class ModerationRequest
{
    public required string Prompt { get; init; }

    /// <inheritdoc cref="ToModerationRequest(string)"/>
    public static implicit operator ModerationRequest(string message)
    {
        return ToModerationRequest(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="ModerationRequest"/>. <br/>
    /// Will be converted to a <see cref="ModerationRequest"/>
    /// with a prompt and default settings.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ModerationRequest ToModerationRequest(string message)
    {
        return new ModerationRequest
        {
            Prompt = message,
        };
    }
}