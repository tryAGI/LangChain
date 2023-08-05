namespace LangChain.NET.Base;

public abstract class BaseLangChain : IBaseLangChainParams
{
    private const bool DefaultVerbosity = false;
    
    public bool? Verbose { get; set; }

    public BaseLangChain(IBaseLangChainParams parameters)
    {
        Verbose = parameters.Verbose ?? DefaultVerbosity;
    }
}