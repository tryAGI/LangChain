namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ITextToSpeechModel<in TSettings>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<byte[]> GenerateSpeechAsync(
        string input,
        TSettings? settings = default,
        CancellationToken cancellationToken = default);
}