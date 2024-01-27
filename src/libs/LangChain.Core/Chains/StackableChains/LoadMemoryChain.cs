using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;

namespace LangChain.Chains.StackableChains;

public class LoadMemoryChain: BaseStackableChain
{
    
    private readonly ConversationBufferMemory _chatMemory;
    private readonly string _outputKey;

    public LoadMemoryChain(ConversationBufferMemory chatMemory,string outputKey)
    {
        
        _chatMemory = chatMemory;
        _outputKey = outputKey;

        OutputKeys = new[] {_outputKey};
    }

    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        values.Value[_outputKey] = _chatMemory.BufferAsString;
        return Task.FromResult(values);
    }
}