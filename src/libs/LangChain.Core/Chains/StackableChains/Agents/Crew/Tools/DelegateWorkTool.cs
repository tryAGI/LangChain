namespace LangChain.Chains.StackableChains.Agents.Crew.Tools;

public class DelegateWorkTool: CrewAgentTool
{
    private readonly IEnumerable<CrewAgent> _coworkers;

    public DelegateWorkTool(IEnumerable<CrewAgent> coworkers) : base("delegate")
    {
        _coworkers = coworkers;
        Description = 
            $@"Useful to delegate a specific task to one of the
following co-workers: [{string.Join(", ", coworkers.Select(x => $"'{x.Role}'"))}].
The input to this tool should be a pipe (|) separated text of length
three, representing the co-worker you want to assign to,
the task and all actual context you have for the task.
For example, `coworker|task|context`";
    }

    public override string ToolAction(string input)
    {
        var split = input.Split('|');
        var agent = split[0];
        var task = split[1];
        var context = split[2];

        var coworker = _coworkers.First(x => x.Role == agent);
        coworker.Context = context;
        var chain = Chain.Set(task, "task")
                    | coworker;
        var res = chain.Run("result").Result;
        
        return res;
    }
}