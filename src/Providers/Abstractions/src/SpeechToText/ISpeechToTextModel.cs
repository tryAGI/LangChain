namespace LangChain.Providers;

/// <summary>
/// Defines the interface for models that convert speech to text. Implementations of this interface should provide mechanisms to transcribe spoken language into textual form.
/// </summary>
public interface ISpeechToTextModel : IModel<SpeechToTextSettings>
{
    /// <summary>
    /// Asynchronously transcribes audio to text based on the provided request and settings.
    /// </summary>
    /// <param name="request">The speech to text request containing the audio data and any additional information required for transcription.</param>
    /// <param name="settings">Optional settings to customize the transcription process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in a SpeechToTextResponse object.</returns>
    public Task<SpeechToTextResponse> TranscribeAsync(
        SpeechToTextRequest request,
        SpeechToTextSettings? settings = default,
        CancellationToken cancellationToken = default);
}