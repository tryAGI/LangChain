namespace LangChain.Providers;

/// <summary>
/// Defines common model properties.
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

/// <summary>
/// Defines a model with typed settings.
/// </summary>
public interface IModel<TSettings> : IModel
{
    /// <summary>
    /// Defines the settings for the model.
    /// </summary>
    public TSettings? Settings { get; set; }
}
