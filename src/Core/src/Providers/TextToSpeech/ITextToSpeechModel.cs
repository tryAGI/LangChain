namespace LangChain.Providers;

/// <summary>
/// Defines a model that can convert text to speech.
/// </summary>
public interface ITextToSpeechModel : IModel
{
    /// <summary>
    /// Generates speech from text.
    /// </summary>
    Task<TextToSpeechResponse> GenerateSpeechAsync(
        TextToSpeechRequest request,
        TextToSpeechSettings? settings = default,
        CancellationToken cancellationToken = default);
}
