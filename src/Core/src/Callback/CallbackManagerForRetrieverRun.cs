using LangChain.Base;
using LangChain.DocumentLoaders;

namespace LangChain.Callback;

/// <summary>
/// Callback manager for retriever run.
/// </summary>
public class CallbackManagerForRetrieverRun : ParentRunManager, IRunManagerImplementation<CallbackManagerForRetrieverRun>
{
    /// <inheritdoc/>
    public CallbackManagerForRetrieverRun()
    {

    }

    /// <inheritdoc/>
    public CallbackManagerForRetrieverRun(
        string runId,
        List<BaseCallbackHandler> handlers,
        List<BaseCallbackHandler> inheritableHandlers,
        string? parentRunId = null,
        List<string>? tags = null,
        List<string>? inheritableTags = null,
        Dictionary<string, object>? metadata = null,
        Dictionary<string, object>? inheritableMetadata = null)
        : base(runId, handlers, inheritableHandlers, parentRunId, tags, inheritableTags, metadata, inheritableMetadata)
    {
    }

    /// <summary>
    /// Run when retriever ends running.
    /// </summary>
    public async Task HandleRetrieverEndAsync(string query, IReadOnlyCollection<Document> docs)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreRetriever)
            {
                try
                {
                    await handler.HandleRetrieverEndAsync(query, docs.ToList(), RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleRetrieverEnd: {ex}").ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Run when retriever errors.
    /// </summary>
    public async Task HandleRetrieverErrorAsync(Exception error, string query)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreRetriever)
            {
                try
                {
                    await handler.HandleRetrieverErrorAsync(error, query, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleRetrieverError: {ex}").ConfigureAwait(false);
                }
            }
        }
    }
}