namespace LangChain.Providers;

/// <summary>
/// Defines a common model properties.
/// </summary>
public interface IModel
{
    /// <summary>
    /// Id of the model.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Total usage of current model/provider.
    /// </summary>
    Usage Usage { get; }

    void AddUsage(Usage usage);
}