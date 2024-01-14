using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Chains.LLM;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Chains.StackableChains.ReAct;
using LangChain.Memory;
using LangChain.Providers;
using static LangChain.Chains.Chain;

namespace LangChain.Chains.StackableChains.Agents.Crew;

public class CrewAgent : BaseStackableChain
{
    public event Action<string> ReceivedTask=delegate{};
    public event Action<string,string> CalledAction = delegate { };
    public event Action<string> ActionResult = delegate { };
    public event Action<string> Answered = delegate { };

    public string Role { get; }
    public string Goal { get; }
    public string? Backstory { get; }
    private readonly IChatModel _model;
    private readonly List<string> _actionsHistory;
    public bool UseMemory { get; set; }=false;
    public bool UseCache { get; set; }
    private IChainValues _currentValues;
    private Dictionary<string, CrewAgentTool> _tools=new Dictionary<string, CrewAgentTool>();

    private StackChain? _chain=null;
    private readonly List<string> _memory;
    private int _maxActions=5;

    public CrewAgent(IChatModel model, string role, string goal, string? backstory = "")
    {
        Role = role;
        Goal = goal;
        Backstory = backstory;
        _model = model;

        InputKeys = new[] {"task"};
        OutputKeys = new[] {"result"};

        _actionsHistory = new List<string>();
        _memory = new List<string>();
    }

    public void AddTools(IEnumerable<CrewAgentTool> tools)
    {
        _tools = tools
            .Where(x => !_tools.ContainsKey(x.Name))
            .ToDictionary(x => x.Name, x => x);
        InitializeChain();
    }

    public string? Context { get; set; } = null;

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
                    | Do(x => _actionsHistory.Add((x["text"] as string)))
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


    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var task = values.Value[InputKeys[0]] as string;
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

            var res = await chain!.Run<object>(OutputKeys[0]);
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
                var toolRes = tool.ToolAction(action.ActionInput);
                ActionResult(toolRes);
                _actionsHistory.Add("Observation: " + toolRes);
                _actionsHistory.Add("Thought:");

                continue;
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

        throw new Exception($"Max actions exceeded({_maxActions})");
    }
}