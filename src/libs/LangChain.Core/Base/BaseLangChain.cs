namespace LangChain.Base;

/// <inheritdoc />
public abstract class BaseLangChain : IBaseLangChainParams
{
    private const bool DefaultVerbosity = false;

    /// <summary>
    /// 
    /// </summary>
    public bool? Verbose { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    protected BaseLangChain(IBaseLangChainParams parameters)
    {
        parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

        Verbose = parameters.Verbose ?? DefaultVerbosity;
    }
}