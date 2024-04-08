using LangChain.Abstractions.Schema;
using LangChain.Databases;
using LangChain.Providers;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class RetrieveDocumentsChain : BaseStackableChain
{
    private readonly IVectorDatabase _vectorDatabase;
    private readonly IEmbeddingModel _embeddingModel;
    private readonly int _amount;

    /// <inheritdoc/>
    public RetrieveDocumentsChain(
        IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        string inputKey = "query",
        string outputKey = "documents",
        int amount = 4)
    {
        _vectorDatabase = vectorDatabase;
        _embeddingModel = embeddingModel;
        _amount = amount;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var retriever = _vectorDatabase.AsRetriever(_embeddingModel);
        retriever.K = _amount;

        var query = values.Value[InputKeys[0]].ToString() ?? string.Empty;
        var results = await retriever.GetRelevantDocumentsAsync(query).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = results.ToList();
        return values;
    }
}