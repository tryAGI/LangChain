using LangChain.Base;

namespace LangChain.Callback;

public class CallbackManagerForRetrieverRun : BaseRunManager
{
    public CallbackManagerForRetrieverRun(
        string runId,
        List<BaseCallbackHandler> handlers,
        List<BaseCallbackHandler> inheritableHandlers,
        string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public async Task HandleRetrieverEndAsync(string query)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreRetriever)
            {
                try
                {
                    await handler.HandleRetrieverEndAsync(query, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleRetrieverEnd: {ex}");
                }
            }
        }
    }

    public async Task HandleRetrieverErrorAsync(Exception error, string query)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreRetriever)
            {
                try
                {
                    await handler.HandleRetrieverErrorAsync(error, query, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleRetrieverError: {ex}");
                }
            }
        }
    }
}