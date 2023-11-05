using LangChain.Docstore;

namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public interface ISource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyCollection<LangChain.Docstore.Document>> LoadAsync(CancellationToken cancellationToken = default);
}