using LangChain.Callback;
using LangChain.Docstore;

namespace LangChain.Retrievers;

/// <summary>
/// Abstract base class for a Document retrieval system.
/// 
/// A retrieval system is defined as something that can take string queries and return
/// the most 'relevant' Documents from some source.
/// <see cref="https://api.python.langchain.com/en/latest/_modules/langchain/schema/retriever.html" />
/// </summary>
public abstract class BaseRetriever
{
	protected abstract Task<IEnumerable<Document>> GetRelevantDocumentsAsync(string query, int k = 4);

	/// <summary>
	/// Retrieve documents relevant to a query.
	/// </summary>
	/// <param name="query">string to find relevant documents for</param>
	/// <param name="runId"></param>
	/// <param name="callbacks"></param>
	/// <returns></returns>
	public virtual async Task<IEnumerable<Document>> GetRelevantDocumentsAsync(string query, string runId, CallbackManager? callbacks = null)
	{
		CallbackManagerForRetrieverRun runManager=null;
        if (callbacks != null)
        {
            runManager = await callbacks.HandleRetrieverStart(this, query, runId);
        }
        
        try
		{
			var docs = await GetRelevantDocumentsAsync(query);
			if(runManager!=null)
			    await runManager.HandleRetrieverEndAsync(query);

			return docs;
		}
		catch (Exception exception)
		{
            if (runManager != null)
                await runManager.HandleRetrieverErrorAsync(exception, query);
			throw;
		}
	}
}

