// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ITextToSpeechModel : IModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TextToSpeechResponse> GenerateSpeechAsync(
        TextToSpeechRequest request,
        TextToSpeechSettings? settings = default,
        CancellationToken cancellationToken = default);
}