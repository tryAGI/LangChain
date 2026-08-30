using System.Globalization;
using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains.ReAct;
using LangChain.Memory;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using System.Reflection;
using LangChain.Chains.StackableChains.Agents.Tools;
using Microsoft.Extensions.AI;
using static LangChain.Chains.Chain;

namespace LangChain.Chains.StackableChains.Agents;

/// <summary>
///
/// </summary>
public class ReActAgentExecutorChain : BaseStackableChain
{
    /// <summary>
    ///
    /// </summary>
    public const string DefaultPrompt =
        @"Answer the following questions as best you can. You have access to the following tools:

{tools}

Use the following format:

Question: the input question you must answer
Thought: you should always think about what to do
Action: the tool name, should be one of [{tool_names}]
Action Input: the input to the tool
Observation: the result of the tool
(this Thought/Action/Action Input/Observation can repeat multiple times)
Thought: I now know the final answer
(no actions before final answer)
Final Answer: the final answer to the original input question
Always add [END] after final answer

Begin!

Question: {input}
Thought:{history}";

    private StackChain? _chain;
    private bool _useCache;
    Dictionary<string, AgentTool> _tools = new();
    private readonly IChatClient _chatClient;
    private readonly string _reActPrompt;
    private readonly int _maxActions;
    private readonly MessageFormatter _messageFormatter;
    private readonly ChatMessageHistory _chatMessageHistory;
    private readonly ConversationBufferMemory _conversationBufferMemory;

    /// <summary>
    ///
    /// </summary>
    /// <param name="chatClient"></param>
    /// <param name="reActPrompt"></param>
    /// <param name="maxActions"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    public ReActAgentExecutorChain(
        IChatClient chatClient,
        string? reActPrompt = null,
        int maxActions = 5,
        string inputKey = "answer",
        string outputKey = "final_answer")
    {
        reActPrompt ??= DefaultPrompt;
        _chatClient = chatClient;
        _reActPrompt = reActPrompt;
        _maxActions = maxActions;

        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

        _messageFormatter = new MessageFormatter
        {
            AiPrefix = "",
            HumanPrefix = "",
            SystemPrefix = ""
        };

        _chatMessageHistory = new ChatMessageHistory()
        {
            // Do not save human messages
            IsMessageAccepted = x => (x.Role != ChatRole.User)
        };

        _conversationBufferMemory = new ConversationBufferMemory(_chatMessageHistory)
        {
            Formatter = _messageFormatter
        };
    }

    private string _userInput = string.Empty;
    private const string ReActAnswer = "answer";
    private void InitializeChain()
    {
        var toolNames = string.Join(",", _tools.Select(x => x.Key));
        var tools = string.Join("\n", _tools.Select(x => $"{x.Value.Name}, {x.Value.Description}"));

        var chain =
            Set(() => _userInput, "input")
            | Set(tools, "tools")
            | Set(toolNames, "tool_names")
            | LoadMemory(_conversationBufferMemory, outputKey: "history")
            | Template(_reActPrompt)
            | Chain.LLM(_chatClient, options: new ChatOptions
            {
                StopSequences = ["Observation", "[END]"],
            }).UseCache(_useCache)
            | UpdateMemory(_conversationBufferMemory, requestKey: "input", responseKey: "text")
            | ReActParser(inputKey: "text", outputKey: ReActAnswer);

        _chain = chain;
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var input = (string)values.Value[InputKeys[0]];
        var valuesChain = new ChainValues();

        _userInput = input;

        if (_chain == null)
        {
            InitializeChain();
        }

        for (int i = 0; i < _maxActions; i++)
        {
            var res = await _chain!.CallAsync(valuesChain, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (res.Value[ReActAnswer] is AgentAction)
            {
                var action = (AgentAction)res.Value[ReActAnswer];
                var tool = _tools[action.Action.ToLower(CultureInfo.InvariantCulture)];
                var toolRes = await tool.ToolTask(action.ActionInput, cancellationToken).ConfigureAwait(false);
                await _conversationBufferMemory.ChatHistory.AddMessage(new ChatMessage(ChatRole.System, "Observation: " + toolRes))
                    .ConfigureAwait(false);
                await _conversationBufferMemory.ChatHistory.AddMessage(new ChatMessage(ChatRole.System, "Thought:"))
                    .ConfigureAwait(false);
                continue;
            }
            else if (res.Value[ReActAnswer] is AgentFinish)
            {
                var finish = (AgentFinish)res.Value[ReActAnswer];
                values.Value[OutputKeys[0]] = finish.Output;
                return values;
            }
        }



        return values;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public ReActAgentExecutorChain UseCache(bool enabled = true)
    {
        _useCache = enabled;
        return this;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="tool"></param>
    /// <returns></returns>
    public ReActAgentExecutorChain UseTool(AgentTool tool)
    {
        tool = tool ?? throw new ArgumentNullException(nameof(tool));

        _tools.Add(tool.Name, tool);
        return this;
    }
}
