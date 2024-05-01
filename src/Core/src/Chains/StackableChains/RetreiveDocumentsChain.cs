using LangChain.Abstractions.Schema;
using LangChain.Databases;
using LangChain.Extensions;
using LangChain.Providers;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class RetrieveDocumentsChain : BaseStackableChain
{
    private readonly IVectorCollection _vectorCollection;
    private readonly IEmbeddingModel _embeddingModel;
    private readonly int _amount;

    /// <inheritdoc/>
    public RetrieveDocumentsChain(
        IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        string inputKey = "query",
        string outputKey = "documents",
        int amount = 4)
    {
        _vectorCollection = vectorCollection;
        _embeddingModel = embeddingModel;
        _amount = amount;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var query = values.Value[InputKeys[0]].ToString() ?? string.Empty;
        var results = await _vectorCollection.GetSimilarDocuments(
            _embeddingModel,
            query,
            amount: _amount,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = results.ToList();
        return values;
    }
}