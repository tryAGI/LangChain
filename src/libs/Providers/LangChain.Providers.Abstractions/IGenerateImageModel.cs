namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IGenerateImageModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Uri> GenerateImageAsUrlAsync(
        string prompt,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Stream> GenerateImageAsStreamAsync(
        string prompt,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<byte[]> GenerateImageAsBytesAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}