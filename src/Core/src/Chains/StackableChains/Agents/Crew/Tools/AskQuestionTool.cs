namespace LangChain.Chains.StackableChains.Agents.Crew.Tools;

/// <summary>
/// 
/// </summary>
public class AskQuestionTool: CrewAgentTool
{
    private readonly IEnumerable<CrewAgent> _coworkers;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coworkers"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AskQuestionTool(IEnumerable<CrewAgent> coworkers) : base("question")
    {
        _coworkers = coworkers ?? throw new ArgumentNullException(nameof(coworkers));
        Description = 
            $@"Useful to ask a question, opinion or take from on
of the following co-workers: [{string.Join(", ", coworkers.Select(x => $"'{x.Role}'"))}].
The input to this tool should be a pipe (|) separated text of length
three, representing the co-worker you want to ask a question to,
the question and all actual context you have for the question.
For example, `coworker|question|context`.";
    }

    /// <inheritdoc />
    public override async Task<string> ToolTask(string input, CancellationToken token = default)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        
        var split = input.Split('|');
        var agent = split[0];
        var task = split[1];
        var context = split[2];

        var coworker = _coworkers.First(x => x.Role == agent);
        coworker.Context = context;
        var chain = Chain.Set(task, "task")
                    | coworker;
        var res = await chain.Run("result").ConfigureAwait(false) ?? string.Empty;
        
        return res;

    }
}