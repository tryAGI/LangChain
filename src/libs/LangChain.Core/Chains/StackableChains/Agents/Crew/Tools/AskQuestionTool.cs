namespace LangChain.Chains.StackableChains.Agents.Crew.Tools;

public class AskQuestionTool: CrewAgentTool
{
    private readonly IEnumerable<CrewAgent> _coworkers;

    public AskQuestionTool(IEnumerable<CrewAgent> coworkers) : base("question")
    {
        _coworkers = coworkers;
        Description = 
            $@"Useful to ask a question, opinion or take from on
of the following co-workers: [{string.Join(", ", coworkers.Select(x => $"'{x.Role}'"))}].
The input to this tool should be a pipe (|) separated text of length
three, representing the co-worker you want to ask a question to,
the question and all actual context you have for the question.
For example, `coworker|question|context`.";
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