using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Providers;

namespace LangChain.Chains.HelperChains;

public class LLMChain:BaseStackableChain
{
    private readonly IChatModel _llm;

    public LLMChain(IChatModel llm, 
        string inputKey="prompt", 
        string outputKey="text"
        )
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
        _llm = llm;
    }

    protected override async Task<IChainValues> InternallCall(IChainValues values)
    {
        var prompt = values.Value[InputKeys[0]].ToString();
        var response=await _llm.GenerateAsync(new ChatRequest(new List<Message>() { prompt.AsSystemMessage() }));
        values.Value[OutputKeys[0]] = response.Messages.Last().Content;
        return values;
    }
}