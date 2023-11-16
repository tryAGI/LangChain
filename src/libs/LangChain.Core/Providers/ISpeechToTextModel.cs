namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ISpeechToTextModel<in TSettings>
{
    /// <summary>
    /// Transcribes audio to text.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> TranscribeAsync(
        Stream stream,
        TSettings? settings = default,
        CancellationToken cancellationToken = default);
}