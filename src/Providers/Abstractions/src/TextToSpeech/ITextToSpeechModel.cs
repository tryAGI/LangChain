// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Defines the interface for models that convert text to speech. Implementations of this interface should provide mechanisms to generate speech from textual input.
/// </summary>
public interface ITextToSpeechModel : IModel
{
    /// <summary>
    /// Asynchronously generates speech from the provided text request using specified settings.
    /// </summary>
    /// <param name="request">The text to speech request containing the text to be converted into speech.</param>
    /// <param name="settings">Optional settings to customize the speech generation process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in a TextToSpeechResponse object.</returns>
    Task<TextToSpeechResponse> GenerateSpeechAsync(
        TextToSpeechRequest request,
        TextToSpeechSettings? settings = default,
        CancellationToken cancellationToken = default);
}