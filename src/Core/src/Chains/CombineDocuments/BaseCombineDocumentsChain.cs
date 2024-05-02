using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Common;
using LangChain.DocumentLoaders;
using LangChain.Schema;

namespace LangChain.Chains.CombineDocuments;

/// <summary>
/// Base interface for chains combining documents.
/// 
/// Subclasses of this chain deal with combining documents in a variety of
/// ways. This base class exists to add some uniformity in the interface these types
/// of chains should expose. Namely, they expect an input key related to the documents
/// to use (default `input_documents`), and then also expose a method to calculate
/// the length of a prompt from documents (useful for outside callers to use to
/// determine whether it's safe to pass a list of documents into this chain or whether
/// that will longer than the context length).
/// </summary>
public abstract class BaseCombineDocumentsChain(
    BaseCombineDocumentsChainInput fields)
    : BaseChain(fields), IChain
{
    /// <summary>
    /// 
    /// </summary>
    public string InputKey { get; } = fields.InputKey;

    /// <summary>
    /// 
    /// </summary>
    public string OutputKey { get; } = fields.OutputKey;

    /// <inheritdoc/>
    public override IReadOnlyList<string> InputKeys => new[] { InputKey };

    /// <inheritdoc/>
    public override IReadOnlyList<string> OutputKeys => new[] { OutputKey };

    /// <summary>
    /// Prepare inputs, call combine docs, prepare outputs.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="runManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager, CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var docs = values.Value["input_documents"];

        //Other keys are assumed to be needed for LLM prediction
        var otherKeys = values.Value
            .Where(kv => kv.Key != InputKey)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var (output, returnDict) = await CombineDocsAsync(
            (docs as List<Document> ?? new List<Document>()), otherKeys, cancellationToken).ConfigureAwait(false);

        returnDict[OutputKey] = output;
        returnDict.TryAddKeyValues(values.Value);

        return new ChainValues(returnDict);
    }

    /// <summary>
    /// Return the prompt length given the documents passed in.
    /// 
    /// This can be used by a caller to determine whether passing in a list
    /// of documents would exceed a certain prompt length. This useful when
    /// trying to ensure that the size of a prompt remains below a certain
    /// context limit.
    /// </summary>
    /// <param name="docs">a list of documents to use to calculate the total prompt length.</param>
    /// <param name="otherKeys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns null if the method does not depend on the prompt length, otherwise the length of the prompt in tokens.</returns>
    public abstract Task<int?> PromptLengthAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Combine documents into a single string.
    /// </summary>
    /// <param name="docs">the documents to combine</param>
    /// <param name="otherKeys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The first element returned is the single string output. The second element returned is a dictionary of other keys to return.</returns>
    public abstract Task<(string Output, Dictionary<string, object> OtherKeys)> CombineDocsAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default);
}