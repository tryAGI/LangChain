// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for speech-to-text requests.
/// </summary>
public class SpeechToTextRequest : IDisposable
{
    public required Stream Stream { get; init; }

    public bool OwnsStream { get; init; } = true;

    /// <inheritdoc cref="ToSpeechToTextRequest(System.IO.Stream)"/>
    public static implicit operator SpeechToTextRequest(Stream stream)
    {
        return ToSpeechToTextRequest(stream);
    }

    /// <inheritdoc cref="ToSpeechToTextRequest(byte[])"/>
    public static implicit operator SpeechToTextRequest(byte[] bytes)
    {
        return ToSpeechToTextRequest(bytes);
    }

    /// <inheritdoc cref="ToSpeechToTextRequest(string)"/>
    public static implicit operator SpeechToTextRequest(string path)
    {
        return ToSpeechToTextRequest(path);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="SpeechToTextRequest"/>. <br/>
    /// Will be converted to a <see cref="SpeechToTextRequest"/>
    /// with a stream and default settings.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static SpeechToTextRequest ToSpeechToTextRequest(Stream stream)
    {
        return new SpeechToTextRequest
        {
            Stream = stream,
        };
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="SpeechToTextRequest"/>. <br/>
    /// Will be converted to a <see cref="SpeechToTextRequest"/>
    /// with a MemoryStream of current bytes and default settings.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static SpeechToTextRequest ToSpeechToTextRequest(byte[] bytes)
    {
        return new SpeechToTextRequest
        {
            Stream = new MemoryStream(bytes),
        };
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="SpeechToTextRequest"/>. <br/>
    /// Will be converted to a <see cref="SpeechToTextRequest"/>
    /// with a FileStream of this path and default settings.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SpeechToTextRequest ToSpeechToTextRequest(string path)
    {
        return new SpeechToTextRequest
        {
            Stream = new FileStream(path, FileMode.Open, FileAccess.Read),
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && OwnsStream)
        {
            Stream.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}