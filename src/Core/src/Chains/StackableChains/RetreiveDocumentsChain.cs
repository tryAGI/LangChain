using LangChain.Abstractions.Schema;
using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class RetrieveDocumentsChain : BaseStackableChain
{
    private readonly VectorStoreCollection<string, LangChainDocumentRecord> _vectorCollection;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly int _amount;

    /// <inheritdoc/>
    public RetrieveDocumentsChain(
        VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string inputKey = "query",
        string outputKey = "documents",
        int amount = 4)
    {
        _vectorCollection = vectorCollection;
        _embeddingGenerator = embeddingGenerator;
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
            _embeddingGenerator,
            query,
            amount: _amount,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = results.ToList();
        return values;
    }
}
