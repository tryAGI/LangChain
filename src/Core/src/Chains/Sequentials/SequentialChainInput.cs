using LangChain.Abstractions.Chains.Base;
using LangChain.Base;
using LangChain.Callback;

namespace LangChain.Chains.Sequentials;

/// <summary>
/// 
/// </summary>
public class SequentialChainInput : IChainInputs
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<IChain> Chains { get; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string> InputVariables { get; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string>? OutputVariables { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool ReturnAll { get; }

    /// <inheritdoc/>
    public bool Verbose { get; set; }

    /// <inheritdoc/>
    public ICallbacks? Callbacks { get; set; }

    /// <inheritdoc/>
    public List<string> Tags { get; set; } = new();

    /// <inheritdoc/>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chains"></param>
    /// <param name="inputVariables"></param>
    /// <param name="outputVariables"></param>
    /// <param name="returnAll"></param>
    public SequentialChainInput(IChain[] chains,
        string[] inputVariables,
        string[]? outputVariables = null,
        bool returnAll = false)
    {
        Chains = chains;
        InputVariables = inputVariables;
        OutputVariables = outputVariables;
        ReturnAll = returnAll;
    }
}