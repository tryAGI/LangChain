using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains.Agents;

public class GroupChat:BaseStackableChain
{
    private readonly IList<AgentExecutorChain> _agents;
    
    private readonly string _stopPhrase;
    private readonly int _messagesLimit;
    private readonly string _inputKey;
    private readonly string _outputKey;
    

    int _currentAgentId=0;
    private readonly ConversationBufferMemory _conversationBufferMemory;


    public bool ThrowOnLimit { get; set; } = false;
    public GroupChat(IList<AgentExecutorChain> agents, string? stopPhrase=null, int messagesLimit=10, string inputKey="input", string outputKey="output")
    {
        _agents = agents;
        
        _stopPhrase = stopPhrase;
        _messagesLimit = messagesLimit;
        _inputKey = inputKey;
        _outputKey = outputKey;
        _conversationBufferMemory = new ConversationBufferMemory(new ChatMessageHistory()) { AiPrefix = "", HumanPrefix = "", SystemPrefix = "", SaveHumanMessages = false };
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

    }

    public IReadOnlyList<Message> GetHistory()
    {
        return _conversationBufferMemory.ChatHistory.Messages;
    }


    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        
        await _conversationBufferMemory.Clear().ConfigureAwait(false);
        foreach (var agent in _agents)
        {
            agent.SetHistory("");
        }
        var firstAgent = _agents[0];
        var firstAgentMessage = (string)values.Value[_inputKey];
        await _conversationBufferMemory.ChatHistory.AddMessage(new Message($"{firstAgent.Name}: {firstAgentMessage}",
            MessageRole.System)).ConfigureAwait(false);
        int messagesCount = 1;
        while (messagesCount<_messagesLimit)
        {
            var agent = GetNextAgent();
            agent.SetHistory(_conversationBufferMemory.BufferAsString+"\n"+$"{agent.Name}:");
            var res = await agent.CallAsync(values).ConfigureAwait(false);
            var message = (string)res.Value[agent.OutputKeys[0]];
            if (message.Contains(_stopPhrase))
            {
                break;
            }

            if (!agent.IsObserver)
            {
                await _conversationBufferMemory.ChatHistory.AddMessage(new Message($"{agent.Name}: {message}",
                                       MessageRole.System)).ConfigureAwait(false);
            }
        }

        var result = _conversationBufferMemory.ChatHistory.Messages.Last();
        messagesCount = _conversationBufferMemory.ChatHistory.Messages.Count;
        if (ThrowOnLimit && messagesCount >= _messagesLimit)
        {
            throw new Exception("Messages limit reached");
        }
        values.Value.Add(_outputKey, result);
        return values;

    }

    AgentExecutorChain GetNextAgent()
    {
        _currentAgentId++;
        if (_currentAgentId >= _agents.Count)
            _currentAgentId = 0;
        return _agents[_currentAgentId];
    }
}