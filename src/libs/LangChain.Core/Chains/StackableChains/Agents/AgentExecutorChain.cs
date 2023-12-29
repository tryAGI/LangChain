using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains.Agents;

public class AgentExecutorChain: BaseStackableChain
{
    public string HistoryKey { get; }
    private readonly BaseStackableChain _originalChain;
    
    private BaseStackableChain _chainWithHistory;

    public string Name { get; private set; }

    /// <summary>
    /// Messages of this agent will not be added to the history
    /// </summary>
    public bool IsObserver { get; set; } = false;

    public AgentExecutorChain(BaseStackableChain originalChain, string name, string historyKey="history", 
        string outputKey = "final_answer")
    {
        Name = name;
        HistoryKey = historyKey;
        _originalChain = originalChain;
        
        InputKeys = new[] { historyKey};
        OutputKeys = new[] { outputKey };

        SetHistory("");
    }

    public void SetHistory(string history)
    {

        _chainWithHistory = 
            Chain.Set(history, HistoryKey)
            |_originalChain;
    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var res=await _chainWithHistory.CallAsync(values);
        return res;
    }
}