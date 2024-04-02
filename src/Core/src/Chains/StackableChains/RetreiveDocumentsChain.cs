using LangChain.Abstractions.Schema;
using LangChain.Databases;
using LangChain.Indexes;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class RetrieveDocumentsChain : BaseStackableChain
{
    private readonly VectorStoreIndexWrapper _index;
    private readonly int _amount;

    /// <inheritdoc/>
    public RetrieveDocumentsChain(VectorStoreIndexWrapper index, string inputKey = "query", string outputKey = "documents", int amount = 4)
    {
        _index = index;
        _amount = amount;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var retriever = _index.Store.AsRetriever();
        retriever.K = _amount;

        var query = values.Value[InputKeys[0]].ToString() ?? string.Empty;
        var results = await retriever.GetRelevantDocumentsAsync(query).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = results.ToList();
        return values;
    }
}