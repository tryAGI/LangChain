// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public abstract class Model(string id) : IModel
{
    #region Fields

    private readonly object _usageLock = new();

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string Id { get; } = id ?? throw new ArgumentNullException(nameof(id));
    
    /// <inheritdoc/>
    public Usage Usage { get; protected set; }
    
    #endregion

    #region Methods

    public void AddUsage(Usage usage)
    {
        lock (_usageLock)
        {
            Usage += usage;
        }
    }

    #endregion
}