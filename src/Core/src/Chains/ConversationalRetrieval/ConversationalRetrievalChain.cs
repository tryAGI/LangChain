using LangChain.Callback;
using LangChain.Chains.CombineDocuments;
using LangChain.DocumentLoaders;
using LangChain.Providers;

namespace LangChain.Chains.ConversationalRetrieval;

/// <summary>
/// Chain for having a conversation based on retrieved documents.
/// 
/// This chain takes in chat history (a list of messages) and new questions,
/// and then returns an answer to that question.
/// The algorithm for this chain consists of three parts:
/// 
/// 1. Use the chat history and the new question to create a "standalone question".
/// This is done so that this question can be passed into the retrieval step to fetch
/// relevant documents. If only the new question was passed in, then relevant context
/// may be lacking. If the whole conversation was passed into retrieval, there may
/// be unnecessary information there that would distract from retrieval.
/// 
/// 2. This new question is passed to the retriever and relevant documents are
/// returned.
/// 
/// 3. The retrieved documents are passed to an LLM along with either the new question
/// (default behavior) or the original question and chat history to generate a final
/// response.
/// </summary>
public class ConversationalRetrievalChain(ConversationalRetrievalChainInput fields)
    : BaseConversationalRetrievalChain(fields)
{
    /// <inheritdoc/>
    public override string ChainType() => "conversational_retrieval";

    /// <inheritdoc/>
    protected override async Task<List<Document>> GetDocsAsync(
        string question,
        Dictionary<string, object> inputs,
        CallbackManagerForChainRun? runManager = null)
    {
        var docs = await fields.Retriever.GetRelevantDocumentsAsync(
            question,
            callbacks: runManager?.ToCallbacks()).ConfigureAwait(false);

        return ReduceTokensBelowLimit(docs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="docs"></param>
    /// <returns></returns>
    public List<Document> ReduceTokensBelowLimit(IEnumerable<Document> docs)
    {
        var docsList = docs.ToList();
        var numDocs = docsList.Count;

        if (fields.MaxTokensLimit != null &&
            fields.CombineDocsChain is StuffDocumentsChain stuffDocumentsChain &&
            stuffDocumentsChain.LlmChain.Llm is ISupportsCountTokens counter)
        {
            var tokens = docsList.Select(doc => counter.CountTokens(doc.PageContent)).ToArray();
            var tokenCount = tokens.Sum();

            while (tokenCount > fields.MaxTokensLimit)
            {
                numDocs -= 1;
                tokenCount -= tokens[numDocs];
            }
        }

        return docsList.Take(numDocs).ToList();
    }
}