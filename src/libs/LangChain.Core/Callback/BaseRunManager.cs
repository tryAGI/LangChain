using LangChain.Base;

namespace LangChain.Callback;

/// <summary>
/// 
/// </summary>
public abstract class BaseRunManager
{
    /// <summary> </summary>
    public string RunId { get; }

    /// <summary> </summary>
    protected List<BaseCallbackHandler> Handlers { get; }

    /// <summary> </summary>
    protected List<BaseCallbackHandler> InheritableHandlers { get; }

    /// <summary> </summary>
    protected string? ParentRunId { get; }

    /// <summary> </summary>
    protected List<string> Tags { get; }

    /// <summary> </summary>
    protected List<string> InheritableTags { get; }

    /// <summary> </summary>
    protected Dictionary<string, object> Metadata { get; }

    /// <summary> </summary>
    protected Dictionary<string, object> InheritableMetadata { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="runId"></param>
    /// <param name="handlers"></param>
    /// <param name="inheritableHandlers"></param>
    /// <param name="parentRunId"></param>
    /// <param name="tags"></param>
    /// <param name="inheritableTags"></param>
    /// <param name="metadata"></param>
    /// <param name="inheritableMetadata"></param>
    protected BaseRunManager(
        string runId,
        List<BaseCallbackHandler> handlers,
        List<BaseCallbackHandler> inheritableHandlers,
        string? parentRunId = null,
        List<string>? tags = null,
        List<string>? inheritableTags = null,
        Dictionary<string, object>? metadata = null,
        Dictionary<string, object>? inheritableMetadata = null)
    {
        RunId = runId;
        Handlers = handlers;
        InheritableHandlers = inheritableHandlers;
        Tags = tags ?? new();
        InheritableTags = inheritableTags ?? new();
        Metadata = metadata ?? new();
        InheritableMetadata = inheritableMetadata ?? new();
        ParentRunId = parentRunId;
    }

    /// <inheritdoc/>
    protected BaseRunManager()
        : this(
            runId: Guid.NewGuid().ToString("N"),
            handlers: new(),
            inheritableHandlers: new())
    {
    }

    /// <summary>
    /// Run when text is received.
    /// </summary>
    /// <param name="text">The received text.</param>
    public async Task HandleText(string text)
    {
        foreach (var handler in Handlers)
        {
            try
            {
                await handler.HandleTextAsync(text, RunId, ParentRunId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleText: {ex}").ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Return a manager that doesn't perform any operations.
    /// TODO: (static abstract not supported by some target runtimes)
    /// </summary>
    public static T GetNoopManager<T>() where T : IRunManagerImplementation<T>, new()
    {
        return new T();
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThis"></typeparam>
public interface IRunManagerImplementation<TThis> where TThis : IRunManagerImplementation<TThis>, new();
