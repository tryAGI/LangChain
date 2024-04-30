using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Agents;

/// <summary>
/// 
/// </summary>
public class AgentExecutorChain : BaseStackableChain
{
    /// <summary>
    /// 
    /// </summary>
    public string HistoryKey { get; }
    
    private readonly BaseStackableChain _originalChain;
    
    private BaseStackableChain? _chainWithHistory;

    /// <summary>
    /// Messages of this agent will not be added to the history
    /// </summary>
    public bool IsObserver { get; set; }

    /// <inheritdoc/>
    public AgentExecutorChain(
        BaseStackableChain originalChain,
        string name,
        string historyKey = "history", 
        string outputKey = "final_answer")
    {
        Name = name;
        HistoryKey = historyKey;
        _originalChain = originalChain;
        
        InputKeys = new[] { historyKey };
        OutputKeys = new[] { outputKey };

        SetHistory("");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="history"></param>
    public void SetHistory(string history)
    {
        _chainWithHistory =
            Chain.Set(history, HistoryKey) |
            _originalChain;
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        if (_chainWithHistory == null)
        {
            throw new InvalidOperationException("History is not set");
        }
        
        return await _chainWithHistory.CallAsync(values, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}