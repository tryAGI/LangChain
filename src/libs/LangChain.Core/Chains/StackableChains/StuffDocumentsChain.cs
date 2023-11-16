using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Docstore;
using LangChain.Prompts;

namespace LangChain.Chains.HelperChains;

public class StuffDocumentsChain : BaseStackableChain
{
    public string DocumentsSeparator { get; set; } = "\n\n";

    public string Format { get; set; } = "{document}";
    public string FormatKey { get; set; } = "document";

    public StuffDocumentsChain(string inputKey = "documents", string outputKey = "combined")
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

    }

    public StuffDocumentsChain WithSeparator(string separator)
    {
        DocumentsSeparator = separator;
        return this;
    }

    public StuffDocumentsChain WithFormat(string format, string key = "document")
    {
        Format = format;
        FormatKey = key;
        return this;
    }

    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        var documentsObject = values.Value[InputKeys[0]];
        if (documentsObject is not List<Document> docs)
        {
            throw new ArgumentException($"{InputKeys[0]} is not a list of documents");
        }

        var docStrings = new List<string>();
        foreach (var doc in docs)
        {
            var docString = PromptTemplate.InterpolateFStringSafe(Format, new Dictionary<string, object> { { FormatKey, doc.PageContent } });
            docStrings.Add(docString);
        }

        var docsString = String.Join(DocumentsSeparator, docStrings);

        values.Value[OutputKeys[0]] = docsString;
        return Task.FromResult(values);
    }
}