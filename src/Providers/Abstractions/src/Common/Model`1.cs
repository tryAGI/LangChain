// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public abstract class Model<TSettings>(string id) : Model(id), IModel<TSettings>
{
    /// <inheritdoc cref="IModel{TSettings}.Settings"/>
    public TSettings? Settings { get; set; }
}