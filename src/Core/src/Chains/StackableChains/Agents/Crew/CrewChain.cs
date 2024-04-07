using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.Chains.StackableChains.Agents.Crew;

/// <summary>
/// 
/// </summary>
/// <param name="allAgents"></param>
/// <param name="manager"></param>
/// <param name="inputKey"></param>
/// <param name="outputKey"></param>
public class CrewChain(
    IEnumerable<CrewAgent> allAgents,
    CrewAgent manager,
    string inputKey = "text",
    string outputKey = "text")
    : BaseStackableChain
{
    /// <summary>
    /// 
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var taskText = values.Value[inputKey].ToString() ?? string.Empty;
        var task = new AgentTask(manager, taskText);

        // add delegation tools
        if (allAgents.Count()>1)
        {
            task.Tools.Add(new AskQuestionTool(allAgents.Except(new[] { task.Agent })));
            task.Tools.Add(new DelegateWorkTool(allAgents.Except(new[] { task.Agent })));
        }
        
        string res;
        try
        {
            res = await task.ExecuteAsync(Context).ConfigureAwait(false);
            Context = res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred during task execution: {ex.Message}");
            return values; // Return the original values if an exception occurs
        }

        values.Value[outputKey] = res;
        return values;
    }
}