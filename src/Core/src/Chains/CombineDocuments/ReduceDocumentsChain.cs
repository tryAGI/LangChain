using LangChain.Common;
using LangChain.Sources;

namespace LangChain.Chains.CombineDocuments;

/// <summary>
/// Combine documents by recursively reducing them.
/// <see cref="ReduceDocumentsChainInput.CombineDocumentsChain"/> is ALWAYS provided. This is final chain that is called.
/// We pass all previous results to this chain, and the output of this chain is
/// returned as a final result.
/// <see cref="ReduceDocumentsChainInput.CollapseDocumentsChain"/> is used if the documents passed in are too many to all
/// be passed to <see cref="ReduceDocumentsChainInput.CombineDocumentsChain"/> in one go. In this case,
/// <see cref="ReduceDocumentsChainInput.CollapseDocumentsChain"/> is called recursively on as big of groups of documents
/// as are allowed.
/// </summary>
public class ReduceDocumentsChain : BaseCombineDocumentsChain
{
    private readonly ReduceDocumentsChainInput _input;

    /// <summary>
    /// Combine documents by recursively reducing them.
    /// <see cref="ReduceDocumentsChainInput.CombineDocumentsChain"/> is ALWAYS provided. This is final chain that is called.
    /// We pass all previous results to this chain, and the output of this chain is
    /// returned as a final result.
    /// <see cref="ReduceDocumentsChainInput.CollapseDocumentsChain"/> is used if the documents passed in are too many to all
    /// be passed to <see cref="ReduceDocumentsChainInput.CombineDocumentsChain"/> in one go. In this case,
    /// <see cref="ReduceDocumentsChainInput.CollapseDocumentsChain"/> is called recursively on as big of groups of documents
    /// as are allowed.
    /// </summary>
    /// <param name="input"></param>
    public ReduceDocumentsChain(ReduceDocumentsChainInput input) : base(input)
    {
        _input = input;
    }

    /// <inheritdoc/>
    public override string ChainType() => "reduce_documents_chain";

    /// <inheritdoc/>
    public override Task<int?> PromptLengthAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<int?>(null);
    }

    /// <summary>
    /// Combine multiple documents recursively.
    /// </summary>
    /// <param name="docs">List of documents to combine, assumed that each one is less than <see cref="ReduceDocumentsChainInput.TokenMax"/>.</param>
    /// <param name="otherKeys">additional parameters to be passed to LLM calls (like other input variables besides the documents)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The first element returned is the single string output.
    /// The second element returned is a dictionary of other keys to return.
    /// </returns>
    public override async Task<(string Output, Dictionary<string, object> OtherKeys)> CombineDocsAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default)
    {
        otherKeys = otherKeys ?? throw new ArgumentNullException(nameof(otherKeys));

        var tokenMax = otherKeys.TryGetValue("token_max", out var key) ? (int?)key : null;
        var (resultDocs, _) = await CollapseAsync(docs, otherKeys, tokenMax, cancellationToken).ConfigureAwait(false);

        var result = await _input.CombineDocumentsChain.CombineDocsAsync(resultDocs, otherKeys, cancellationToken).ConfigureAwait(false);

        return result;
    }

    private async Task<(List<Document>, Dictionary<string, object>)> CollapseAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        int? tokenMax = null,
        CancellationToken cancellationToken = default)
    {
        var resultDocs = docs.ToList();
        var numTokens = await _input.CombineDocumentsChain.PromptLengthAsync(resultDocs, otherKeys, cancellationToken).ConfigureAwait(false);

        tokenMax ??= _input.TokenMax;

        while (numTokens != null && numTokens > tokenMax)
        {
            var newResultDocList = await SplitListOfDocsAsync(resultDocs, tokenMax, otherKeys).ConfigureAwait(false);
            resultDocs = new List<Document>();
            foreach (var list in newResultDocList)
            {
                var newDoc = await CollapseDocsAsync(list, otherKeys, cancellationToken).ConfigureAwait(false);
                resultDocs.Add(newDoc);
            }

            numTokens = await _input.CombineDocumentsChain.PromptLengthAsync(resultDocs, otherKeys, cancellationToken).ConfigureAwait(false);
        }

        return (resultDocs, new Dictionary<string, object>());
    }

    /// <summary>
    /// Split Documents into subsets that each meet a cumulative length constraint.
    /// </summary>
    /// <param name="docs">The full list of Documents.</param>
    /// <param name="tokenMax">The maximum cumulative length of any subset of Documents.</param>
    /// <param name="otherKeys">Arbitrary additional keyword params to pass to each call of the length_func.</param>
    /// <returns></returns>
    private async Task<List<List<Document>>> SplitListOfDocsAsync(
        IReadOnlyList<Document> docs,
        int? tokenMax = null,
        IReadOnlyDictionary<string, object>? otherKeys = null)
    {
        otherKeys ??= new Dictionary<string, object>();

        var newResultDocList = new List<List<Document>>();
        var subResultDocs = new List<Document>();

        foreach (var doc in docs)
        {
            subResultDocs.Add(doc);
            var numTokens = await _input.CombineDocumentsChain.PromptLengthAsync(subResultDocs, otherKeys).ConfigureAwait(false);
            if (numTokens > tokenMax)
            {
                if (subResultDocs.Count == 1)
                {
                    throw new InvalidOperationException(
                        "A single document was longer than the context length, we cannot handle this.");
                }

                newResultDocList.Add(subResultDocs.Take(subResultDocs.Count - 1).ToList());
                subResultDocs = subResultDocs.Skip(1).ToList();
            }
        }

        newResultDocList.Add(subResultDocs);

        return newResultDocList;
    }

    /// <summary>
    /// Execute a collapse function on a set of documents and merge their metadatas.
    /// </summary>
    /// <param name="docs">A list of Documents to combine.</param>
    /// <param name="otherKeys">Arbitrary additional keyword params to pass to each call of the combine_document_func.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A single Document with the output of combine_document_func for the page content
    /// and the combined metadata's of all the input documents. All metadata values
    /// are strings, and where there are overlapping keys across documents the
    /// values are joined by ", "
    /// </returns>
    private async Task<Document> CollapseDocsAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object>? otherKeys = null,
        CancellationToken cancellationToken = default)
    {
        var dictionary = new Dictionary<string, object>
        {
            ["input_documents"] = docs,
        };

        dictionary.TryAddKeyValues(otherKeys ?? new Dictionary<string, object>());

        var collapseChain = _input.CollapseDocumentsChain ?? _input.CombineDocumentsChain;
        var result = await collapseChain.RunAsync(dictionary, cancellationToken: cancellationToken).ConfigureAwait(false);

        var combinedMetadata = docs[0].Metadata.ToDictionary(
            kv => kv.Key,
            kv => kv.Value?.ToString() ?? String.Empty);

        foreach (var doc in docs.Skip(1))
        {
            foreach (var kv in doc.Metadata)
            {
                if (combinedMetadata.ContainsKey(kv.Key))
                {
                    combinedMetadata[kv.Key] += kv.Value;
                }
                else
                {
                    combinedMetadata[kv.Key] = kv.Value.ToString() ?? string.Empty;
                }
            }
        }

        var metadata = combinedMetadata.ToDictionary(kv => kv.Key, kv => (object)kv.Value);

        return new Document(result, metadata);
    }
}