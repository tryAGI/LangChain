namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ISpeechToTextModel : IModel<SpeechToTextSettings>
{
    /// <summary>
    /// Transcribes audio to text.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<SpeechToTextResponse> TranscribeAsync(
        SpeechToTextRequest request,
        SpeechToTextSettings? settings = default,
        CancellationToken cancellationToken = default);
}