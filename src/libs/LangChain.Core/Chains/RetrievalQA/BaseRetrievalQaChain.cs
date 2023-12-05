using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Chains.CombineDocuments;
using LangChain.Docstore;
using LangChain.Schema;

namespace LangChain.Chains.RetrievalQA;

/// <summary>
/// Base class for question-answering chains.
/// </summary>
/// <param name="fields"></param>
public abstract class BaseRetrievalQaChain(BaseRetrievalQaChainInput fields) : BaseChain(fields), IChain
{
    private readonly string _inputKey = fields.InputKey;
    private readonly string _outputKey = fields.OutputKey;
    private readonly bool _returnSourceDocuments = fields.ReturnSourceDocuments;
    private readonly BaseCombineDocumentsChain _combineDocumentsChain = fields.CombineDocumentsChain;

    private const string SourceDocuments = "source_documents";

    public CallbackManager? CallbackManager { get; set; }

    public override string[] InputKeys => new[] { _inputKey };
    public override string[] OutputKeys => fields.ReturnSourceDocuments
        ? new[] { _outputKey, SourceDocuments }
        : new[] { _outputKey };

    /// <summary>
    /// Run get_relevant_text and llm on input query.
    /// 
    /// If chain has 'return_source_documents' as 'True', returns
    /// the retrieved documents as well under the key 'source_documents'.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="runManager"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task<IChainValues> CallAsync(
        IChainValues values,
        CallbackManagerForChainRun? runManager)
    {
        runManager ??= BaseRunManager.GetNoopManager<CallbackManagerForChainRun>();

        var question = values.Value[_inputKey].ToString();

        var docs = (await GetDocsAsync(question, runManager).ConfigureAwait(false)).ToList();

        var input = new Dictionary<string, object>
        {
            ["input_documents"] = docs,
            [_inputKey] = question
        };

        var answer = await _combineDocumentsChain.Run(input).ConfigureAwait(false);

        var output = new Dictionary<string, object>
        {
            [_outputKey] = answer
        };

        if (_returnSourceDocuments)
        {
            output.Add(SourceDocuments, docs);
        }

        return new ChainValues(output);
    }

    /// <summary>
    /// Get documents to do question answering over.
    /// </summary>
    /// <param name="question"></param>
    /// <param name="runManager"></param>
    public abstract Task<IEnumerable<Document>> GetDocsAsync(string question, CallbackManagerForChainRun runManager);
}