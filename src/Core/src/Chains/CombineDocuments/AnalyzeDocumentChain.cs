using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Extensions;
using LangChain.Sources;
using LangChain.Schema;
using LangChain.Splitters.Text;

namespace LangChain.Chains.CombineDocuments;

/// <summary>
/// 
/// </summary>
/// <param name="fields"></param>
public class AnalyzeDocumentChain(AnalyzeDocumentsChainInput fields) : BaseChain(fields), IChain
{
    private readonly string _inputKey = fields.InputKey;
    private readonly string _outputKey = fields.OutputKey;

    private readonly ITextSplitter _textSplitter = fields.Splitter ?? new RecursiveCharacterTextSplitter();
    private readonly BaseCombineDocumentsChain _combineDocumentsChain = fields.CombineDocumentsChain;

    /// <inheritdoc/>
    public override string ChainType() => "analyze_document_chain";

    /// <inheritdoc/>
    public override IReadOnlyList<string> InputKeys => new[] { _inputKey };
    
    /// <inheritdoc/>
    public override IReadOnlyList<string> OutputKeys => new[] { _outputKey };

    /// <inheritdoc/>
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager, CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var documents = values.Value[_inputKey];
        var docs = _textSplitter.SplitDocuments(documents as List<Document> ?? new List<Document>()).ToList();

        var otherKeys = values.Value
            .Where(kv => kv.Key != _inputKey)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        otherKeys[_combineDocumentsChain.InputKey] = docs;

        var combined = await _combineDocumentsChain.CallAsync(new ChainValues(otherKeys), cancellationToken: cancellationToken).ConfigureAwait(false);

        return combined;
    }
}