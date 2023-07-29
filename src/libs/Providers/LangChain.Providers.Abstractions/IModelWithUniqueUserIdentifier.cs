namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IModelWithUniqueUserIdentifier
{
    /// <summary>
    /// A unique identifier representing your end-user, which can help provider to monitor and detect abuse.
    /// </summary>
    public string User { get; set; }
}