using LangChain.Base;

namespace LangChain.Callback;

public class BaseRunManager
{
    public string RunId { get; }
    protected List<BaseCallbackHandler> Handlers { get; }
    protected List<BaseCallbackHandler> InheritableHandlers { get; }
    protected string? ParentRunId { get; }

    public BaseRunManager(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
    {
        RunId = runId;
        Handlers = handlers;
        InheritableHandlers = inheritableHandlers;
        ParentRunId = parentRunId;
    }

    public async Task HandleText(string text)
    {
        foreach (var handler in Handlers)
        {
            try
            {
                await handler.HandleTextAsync(text, RunId, ParentRunId);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleText: {ex}");
            }
        }
    }
}