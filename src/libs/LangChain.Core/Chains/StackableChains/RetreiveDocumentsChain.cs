using LangChain.Abstractions.Schema;
using LangChain.Callback;
using System.Numerics;
using LangChain.Indexes;

namespace LangChain.Chains.HelperChains;

public class RetreiveDocumentsChain : BaseStackableChain
{
    private readonly VectorStoreIndexWrapper _index;
    private readonly int _amount;

    public RetreiveDocumentsChain(VectorStoreIndexWrapper index, string inputKey = "query", string outputKey = "documents", int amount = 4)
    {
        _index = index;
        _amount = amount;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var retreiver = _index.Store.AsRetreiver();
        retreiver.K = _amount;

        var query = values.Value[InputKeys[0]].ToString();
        var results = await retreiver.GetRelevantDocumentsAsync(query).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = results.ToList();
        return values;
    }
}