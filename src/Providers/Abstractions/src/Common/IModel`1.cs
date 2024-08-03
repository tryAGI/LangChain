// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Defines a common model properties.
/// </summary>
public interface IModel<TSettings> : IModel
{
    /// <summary>
    /// Defines the settings for the model. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the request. <br/>
    /// If not set, the model will try to use the provider chat settings or default settings in other cases. <br/>
    /// </summary>
    public TSettings? Settings { get; set; }
}