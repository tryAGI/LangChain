using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Chains.StackableChains.ReAct;
using LangChain.Providers;
using static LangChain.Chains.Chain;

namespace LangChain.Chains.StackableChains.Agents.Crew;

/// <summary>
/// 
/// </summary>
public class CrewAgent : BaseStackableChain
{
    /// <summary>
    /// 
    /// </summary>
    public event Action<string> ReceivedTask=delegate{};
    
    /// <summary>
    /// 
    /// </summary>
    public event Action<string,string> CalledAction = delegate { };
    
    /// <summary>
    /// 
    /// </summary>
    public event Action<string> ActionResult = delegate { };
    
    /// <summary>
    /// 
    /// </summary>
    public event Action<string> Answered = delegate { };

    /// <summary>
    /// 
    /// </summary>
    public string Role { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Goal { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Backstory { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseMemory { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseCache { get; set; }
    
    
    private readonly IChatModel _model;
    private readonly List<string> _actionsHistory;
    private Dictionary<string, CrewAgentTool> _tools = new();

    private StackChain? _chain;
    private readonly List<string> _memory;
    private int _maxActions = 5;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="role"></param>
    /// <param name="goal"></param>
    /// <param name="backstory"></param>
    public CrewAgent(
        IChatModel model,
        string role,
        string goal,
        string? backstory = "")
    {
        Role = role;
        Goal = goal;
        Backstory = backstory ?? string.Empty;
        _model = model;

        InputKeys = new[] {"task"};
        OutputKeys = new[] {"result"};

        _actionsHistory = new List<string>();
        _memory = new List<string>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tools"></param>
    public void AddTools(IEnumerable<CrewAgentTool> tools)
    {
        _tools = tools
            .Where(x => !_tools.ContainsKey(x.Name))
            .ToDictionary(x => x.Name, x => x);
        InitializeChain();
    }

    /// <summary>
    /// 
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxActions
    {
        get => _maxActions;
        set => _maxActions = value;
    }

    private string GenerateToolsDescriptions()
    {
        if (_tools.Count==0) return "";
        return string.Join("\n", _tools.Select(x => $"- {x.Value.Name}, {x.Value.Description}\n"));
    }

    private string GenerateToolsNamesList()
    {
        if (_tools.Count == 0) return "";
        return string.Join(", ", _tools.Select(x => x.Key));
    }

    private void InitializeChain()
    {
        string prompt;
        if (UseMemory)
        {
            prompt = Prompts.TaskExecutionWithMemory;
        }
        else
        {
            prompt = Prompts.TaskExecutionWithoutMemory;
        }


        var chain = Set(GenerateToolsDescriptions, "tools")
                    | Set(GenerateToolsNamesList, "tool_names")
                    | Set(Role, "role")
                    | Set(Goal, "goal")
                    | Set(Backstory, "backstory")
                    | Set(() => string.Join("\n", _memory), "memory")
                    | Set(() => string.Join("\n", _actionsHistory), "actions_history")
                    | Template(prompt)
                    | Chain.LLM(_model).UseCache(UseCache)
                    | Do(x => _actionsHistory.Add(x["text"] as string ?? string.Empty))
                    | ReActParser(inputKey: "text", outputKey: OutputKeys[0])
                    | Do(AddToMemory);


        _chain = chain;
    }

    private void AddToMemory(Dictionary<string, object> obj)
    {
        if (!UseMemory) return;
        var res = obj[OutputKeys[0]];
        if (res is AgentFinish a)
        {
            _memory.Add(a.Output);
        }
    }

    /// <inheritdoc />
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var task = values.Value[InputKeys[0]] as string ?? string.Empty;
        _actionsHistory.Clear();
        
        ReceivedTask(task);

        if (Context!=null)
        {
            task += "\n" + "This is the context you are working with:\n"+Context;
        }

        if (_chain == null)
        {
            InitializeChain();
        }
        var chain =
            Set(task, "task")
            | _chain!;
        for (int i = 0; i < _maxActions; i++)
        {
            var res = await chain.Run<object>(OutputKeys[0]).ConfigureAwait(false);
            if (res is AgentAction action)
            {
                CalledAction(action.Action, action.ActionInput);

                if (!_tools.ContainsKey(action.Action))
                {
                    ActionResult("You don't have this tool");
                    _actionsHistory.Add("Observation: You don't have this tool");
                    _actionsHistory.Add("Thought:");
                    continue;
                }

                var tool = _tools[action.Action];
                var toolRes = await tool.ToolTask(action.ActionInput).ConfigureAwait(false);
                ActionResult(toolRes);
                _actionsHistory.Add("Observation: " + toolRes);
                _actionsHistory.Add("Thought:");
            }
            else if (res is AgentFinish finish)
            {
                values.Value.Add(OutputKeys[0], finish.Output);
                if(UseMemory)
                    _memory.Add(finish.Output);

                Answered(finish.Output);
                return values;
            }
        }

        throw new InvalidOperationException($"Max actions exceeded({_maxActions})");
    }
}