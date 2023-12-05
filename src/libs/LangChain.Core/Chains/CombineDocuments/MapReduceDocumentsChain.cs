using LangChain.Chains.LLM;
using LangChain.Common;
using LangChain.Docstore;
using LangChain.Schema;

namespace LangChain.Chains.CombineDocuments;

/// <summary>
/// Combining documents by mapping a chain over them, then combining results.
/// 
/// We first call <see cref="MapReduceDocumentsChain.LlmChain"/> on each document individually, passing in the
/// <see cref="Document.PageContent"/> and any other keys. This is the `map` step.
/// 
/// We then process the results of that `map` step in a `reduce` step. This should
/// be done by a <see cref="MapReduceDocumentsChain.ReduceDocumentsChain"/>.
/// </summary>
public class MapReduceDocumentsChain : BaseCombineDocumentsChain
{
    /// <summary>
    /// Chain to apply to each document individually.
    /// </summary>
    public ILlmChain LlmChain { get; init; }

    /// <summary>
    /// Chain to use to reduce the results of applying `llm_chain` to each doc.
    /// This typically either a ReduceDocumentChain or StuffDocumentChain.
    /// </summary>
    public BaseCombineDocumentsChain ReduceDocumentsChain { get; init; }

    /// <summary>
    /// The variable name in the llm_chain to put the documents in.
    /// If only one variable in the llm_chain, this need not be provided.
    /// </summary>
    /// <returns></returns>
    public string DocumentVariableName { get; init; }

    /// <summary>
    /// Return the results of the map steps in the output.
    /// </summary>
    public bool ReturnIntermediateSteps { get; init; }

    public override string[] OutputKeys { get; }

    public override string ChainType() => "map_reduce_documents_chain";

    public MapReduceDocumentsChain(MapReduceDocumentsChainInput input) : base(input)
    {
        LlmChain = input.LlmChain;
        ReduceDocumentsChain = input.ReduceDocumentsChain;
        ReturnIntermediateSteps = input.ReturnIntermediateSteps;

        DocumentVariableName = ValidateAndGetDocumentKey();

        if (ReturnIntermediateSteps)
        {
            var keys = base.OutputKeys.ToList();
            keys.Add("intermediate_steps");
            OutputKeys = keys.ToArray();
        }
        else
        {
            OutputKeys = base.OutputKeys;
        }

        string ValidateAndGetDocumentKey()
        {
            string documentKey;
            var inputVariable = input.DocumentVariableName;
  
            if (String.IsNullOrEmpty(inputVariable))
            {
                var llmChainVariables = input.LlmChain.InputKeys;
                if (llmChainVariables.Length == 1)
                {
                    documentKey = llmChainVariables[0];
                }
                else
                {
                    throw new ArgumentException(
                        $"{nameof(DocumentVariableName)} must be provided if there are multiple {nameof(LlmChain)}.{nameof(LlmChain.InputKeys)}");
                }
            }
            else
            {
                var llmChainVariables = input.LlmChain.InputKeys;
                if (!llmChainVariables.Contains(inputVariable))
                {
                    throw new ArgumentException(
                        $"{nameof(DocumentVariableName)} {inputVariable} was not found in {nameof(LlmChain)}.{nameof(LlmChain.InputKeys)}: {String.Join(",", LlmChain.InputKeys)}");
                }

                documentKey = inputVariable;
            }

            return documentKey;
        }
    }

    public override async Task<int?> PromptLength(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys)
        => throw new NotImplementedException();

    /// <summary>
    /// Combine documents in a map reduce manner.
    /// 
    /// Combine by mapping first chain over all documents, then reducing the results.
    /// This reducing can be done recursively if needed (if there are many documents).
    /// </summary>
    /// <param name="docs"></param>
    /// <param name="otherKeys"></param>
    /// <returns></returns>
    public override async Task<(string Output, Dictionary<string, object> OtherKeys)> CombineDocsAsync(
        IReadOnlyList<Document> docs,
        IReadOnlyDictionary<string, object> otherKeys)
    {
        var inputs = docs
            .Select(doc =>
            {
                var dictionary = new Dictionary<string, object>
                {
                    [DocumentVariableName] = doc.PageContent
                };

                dictionary.TryAddKeyValues(otherKeys);

                return new ChainValues(dictionary);
            })
            .ToList();

        var mapResults = await LlmChain.ApplyAsync(inputs).ConfigureAwait(false);
        var questionResultKey = LlmChain.OutputKey;

        // this uses metadata from the docs, and the textual results from `results`
        var resultDocs =
            mapResults
                .Select((r, i) => new Document(r.Value[questionResultKey] as string, docs[i].Metadata))
                .ToList();

        var (result, extraReturnDict) = await ReduceDocumentsChain.CombineDocsAsync(resultDocs, otherKeys).ConfigureAwait(false);

        if (ReturnIntermediateSteps)
        {
            var intermediateSteps = mapResults.Select(r => r.Value[questionResultKey]).ToList();
            extraReturnDict["intermediate_steps"] = intermediateSteps;
        }

        return (result, extraReturnDict);
    }
}