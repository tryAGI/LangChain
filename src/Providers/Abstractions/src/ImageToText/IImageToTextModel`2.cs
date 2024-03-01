namespace LangChain.Providers;

/// <summary>
/// Defines a large language model that can be used for chat.
/// </summary>
public interface IImageToTextModel<in TRequest, TResponse, in TSettings> : IImageToTextModel
{
    /// <summary>
    /// Run the LLM on the given prompt and input.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TResponse> GenerateTextFromImageAsync(
        TRequest request,
        TSettings? settings = default,
        CancellationToken cancellationToken = default);
}