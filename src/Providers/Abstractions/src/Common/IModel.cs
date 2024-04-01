namespace LangChain.Providers;

/// <summary>
/// Represents the base interface for models in the LangChain framework. It defines essential properties and methods that all models must implement, including a unique identifier (Id) and usage tracking.
/// Implementing classes are expected to provide specific functionalities and data structures relevant to their domain while adhering to this common structure.
/// </summary>
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