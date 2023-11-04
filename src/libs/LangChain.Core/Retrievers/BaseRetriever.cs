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
	/// <summary>
	/// Optional list of tags associated with the retriever. Defaults to None
	/// These tags will be associated with each call to this retriever,
	/// and passed as arguments to the handlers defined in `callbacks`.
	/// You can use these to eg identify a specific instance of a retriever with its 
	/// use case.
	/// </summary>
	public List<string> Tags { get; set; }
	
	/// <summary>
	/// Optional metadata associated with the retriever. Defaults to None
	/// This metadata will be associated with each call to this retriever,
	/// and passed as arguments to the handlers defined in `callbacks`.
	/// You can use these to eg identify a specific instance of a retriever with its 
	/// use case.
	/// </summary>
	public Dictionary<string, object> Metadata { get; set; }

	protected abstract Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(string query, CallbackManagerForRetrieverRun runManager = null);

	/// <summary>
	/// Retrieve documents relevant to a query.
	/// </summary>
	/// <param name="query">string to find relevant documents for</param>
	/// <param name="runId"></param>
	/// <param name="callbacks"></param>
	/// <param name="verbose"></param>
	/// <param name="tags"></param>
	/// <param name="metadata"></param>
	/// <returns>Relevant documents</returns>
	public virtual async Task<IEnumerable<Document>> GetRelevantDocumentsAsync(
		string query,
		string? runId = null,
		Callbacks? callbacks = null,
		bool verbose = false,
		List<string>? tags = null,
		Dictionary<string, object>? metadata = null)
	{
		var callbackManager = await CallbackManager.Configure(
			callbacks,
			localCallbacks: null,
			verbose: verbose,
			localTags: Tags,
			inheritableTags: tags,
			localMetadata: Metadata,
			inheritableMetadata: metadata);

		var runManager = await callbackManager.HandleRetrieverStart(this, query, runId);
		try
		{
			var docs = await GetRelevantDocumentsCoreAsync(query, runManager);
			await runManager.HandleRetrieverEndAsync(query);

			return docs;
		}
		catch (Exception exception)
		{
			await runManager.HandleRetrieverErrorAsync(exception, query);
			throw;
		}
	}
}

