using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains;

/// <summary>
/// 
/// </summary>
/// <param name="memory"></param>
/// <param name="requestKey"></param>
/// <param name="responseKey"></param>
public class UpdateMemoryChain(
    BaseChatMemory memory,
    string requestKey = "query",
    string responseKey = "text")
    : BaseStackableChain
{
    /// <inheritdoc />
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        await memory.SaveContext(new InputValues(
            new Dictionary<string, object>
            {
                [requestKey] = values.Value[requestKey],
            }),
            new OutputValues(new Dictionary<string, object>
            {
                [responseKey] = values.Value[responseKey],
            })).ConfigureAwait(false);
     
        return values;
    }
}