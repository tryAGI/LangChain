using LangChain.Chains.LLM;
using LangChain.DocumentLoaders;
using LangChain.Prompts;
using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Chains.CombineDocuments;

/// <summary>
/// Chain that combines documents by stuffing into context.
/// 
/// This chain takes a list of documents and first combines them into a single string.
/// It does this by formatting each document into a string with the `document_prompt`
/// and then joining them together with `document_separator`. It then adds that new
/// string to the inputs with the variable name set by `document_variable_name`.
/// Those inputs are then passed to the `LlmChain`.
/// </summary>
public class StuffDocumentsChain : BaseCombineDocumentsChain
{
    /// <summary>
    /// 
    /// </summary>
    public ILlmChain LlmChain { get; }

    private readonly BasePromptTemplate _documentPrompt;
    private readonly string _documentVariableName;
    private readonly string _documentSeparator;

    /// <inheritdoc/>
    public StuffDocumentsChain(StuffDocumentsChainInput input) : base(input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        LlmChain = input.LlmChain;
        _documentPrompt = input.DocumentPrompt;
        _documentSeparator = input.DocumentSeparator;

        var llmChainVariables = LlmChain.Prompt.InputVariables;

        if (input.DocumentVariableName == null)
        {
            _documentVariableName = llmChainVariables.Count == 1
                ? llmChainVariables[0]
                : throw new ArgumentException(
                    "DocumentVariableName must be provided if there are multiple llmChain prompt InputVariables");
        }
        else if (!llmChainVariables.Contains(input.DocumentVariableName))
        {
            throw new ArgumentException(
                $"document_variable_name {input.DocumentVariableName} was not found in llm chain input variables: {String.Join(",", llmChainVariables)}");
        }
        else
        {
            _documentVariableName = input.DocumentVariableName;
        }
    }

    /// <inheritdoc/>
    public override IReadOnlyList<string> InputKeys =>
        base.InputKeys.Concat(LlmChain.InputKeys.Where(k => k != _documentVariableName)).ToArray();

    /// <inheritdoc/>
    public override string ChainType() => "stuff_documents_chain";

    /// <inheritdoc/>
    public override async Task<(string Output, Dictionary<string, object> OtherKeys)> CombineDocsAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default)
    {
        otherKeys = otherKeys ?? throw new ArgumentNullException(nameof(otherKeys));

        var inputs = await GetInputs(docs, otherKeys, cancellationToken).ConfigureAwait(false);
        var predict = await LlmChain.PredictAsync(new ChainValues(inputs.Value), cancellationToken).ConfigureAwait(false);

        return (predict.ToString() ?? string.Empty, new Dictionary<string, object>());
    }

    /// <inheritdoc/>
    public override async Task<int?> PromptLengthAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default)
    {
        otherKeys = otherKeys ?? throw new ArgumentNullException(nameof(otherKeys));

        if (LlmChain.Llm is ISupportsCountTokens supportsCountTokens)
        {
            var inputs = await GetInputs(docs, otherKeys, cancellationToken).ConfigureAwait(false);
            var prompt = await LlmChain.Prompt.FormatPromptValueAsync(inputs, cancellationToken).ConfigureAwait(false);

            return supportsCountTokens.CountTokens(prompt.ToString());
        }

        return null;
    }

    private async Task<InputValues> GetInputs(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys,
        CancellationToken cancellationToken = default)
    {
        var docsString = await GetDocsString(docs, cancellationToken).ConfigureAwait(false);

        var inputs = new Dictionary<string, object>();
        foreach (var kv in otherKeys)
        {
            if (LlmChain.Prompt.InputVariables.Contains(kv.Key))
            {
                inputs[kv.Key] = kv.Value;
            }
        }

        inputs[_documentVariableName] = docsString;

        return new InputValues(inputs);
    }

    private async Task<string> GetDocsString(
        IReadOnlyList<Document> docs,
        CancellationToken cancellationToken = default)
    {
        var docStrings = new List<string>();
        foreach (var doc in docs)
        {
            var docString = await PromptHelpers.FormatDocumentAsync(doc, _documentPrompt, cancellationToken).ConfigureAwait(false);
            docStrings.Add(docString);
        }

        var docsString = String.Join(_documentSeparator, docStrings);
        return docsString;
    }
}