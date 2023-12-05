using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Docstore;
using LangChain.Schema;
using LangChain.TextSplitters;

namespace LangChain.Chains.CombineDocuments;

public class AnalyzeDocumentChain(AnalyzeDocumentsChainInput fields) : BaseChain(fields), IChain
{
    private readonly string _inputKey = fields.InputKey;
    private readonly string _outputKey = fields.OutputKey;

    private readonly TextSplitter _textSplitter = fields.Splitter ?? new RecursiveCharacterTextSplitter();
    private readonly BaseCombineDocumentsChain _combineDocumentsChain = fields.CombineDocumentsChain;

    public override string ChainType() => "analyze_document_chain";

    public override string[] InputKeys => new[] { _inputKey };
    public override string[] OutputKeys => new[] { _outputKey };

    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager)
    {
        var documents = values.Value[_inputKey];
        var docs = _textSplitter.SplitDocuments(documents as List<Document>);

        var otherKeys = values.Value
            .Where(kv => kv.Key != _inputKey)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        otherKeys[_combineDocumentsChain.InputKey] = docs;

        var combined = await _combineDocumentsChain.CallAsync(new ChainValues(otherKeys)).ConfigureAwait(false);

        return combined;
    }
}