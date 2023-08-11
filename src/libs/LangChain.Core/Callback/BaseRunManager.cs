using LangChain.Base;

namespace LangChain.Callback;

/// <summary>
/// 
/// </summary>
public class BaseRunManager
{
    /// <summary>
    /// 
    /// </summary>
    public string RunId { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected List<BaseCallbackHandler> Handlers { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected List<BaseCallbackHandler> InheritableHandlers { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected string? ParentRunId { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="runId"></param>
    /// <param name="handlers"></param>
    /// <param name="inheritableHandlers"></param>
    /// <param name="parentRunId"></param>
    public BaseRunManager(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
    {
        RunId = runId;
        Handlers = handlers;
        InheritableHandlers = inheritableHandlers;
        ParentRunId = parentRunId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
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