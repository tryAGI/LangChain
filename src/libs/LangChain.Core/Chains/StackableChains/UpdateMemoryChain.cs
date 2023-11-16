using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains;

public class UpdateMemoryChain : BaseStackableChain
{
    private readonly BaseChatMemory _memory;
    private readonly string _requestKey;
    private readonly string _responseKey;

    public UpdateMemoryChain(BaseChatMemory memory, string requestKey = "query", string responseKey = "text")
    {
        _memory = memory;
        _requestKey = requestKey;
        _responseKey = responseKey;
    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        await _memory.SaveContext(new InputValues(
                new Dictionary<string, object>() { { _requestKey, values.Value[_requestKey] } }),
            new OutputValues(
                new Dictionary<string, object>() { { _responseKey, values.Value[_responseKey] } }
            ));

     
        return values;
    }
}