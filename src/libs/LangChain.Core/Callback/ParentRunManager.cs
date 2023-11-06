using LangChain.Base;

namespace LangChain.Callback;

/// <summary>
/// Sync Parent Run Manager.
/// </summary>
public class ParentRunManager : BaseRunManager
{
    public ParentRunManager()
    {
        
    }

    public ParentRunManager(
        string runId,
        List<BaseCallbackHandler> handlers,
        List<BaseCallbackHandler> inheritableHandlers,
        string? parentRunId = null,
        List<string>? tags = null,
        List<string>? inheritableTags = null,
        Dictionary<string, object>? metadata = null,
        Dictionary<string,object>? inheritableMetadata = null)
        : base(runId, handlers, inheritableHandlers, parentRunId, tags, inheritableTags, metadata, inheritableMetadata)
    {
    }

    /// <summary>
    /// Get a child callback manager.
    /// </summary>
    /// <param name="tag">The tag for the child callback manager.</param>
    /// <returns>The child callback manager.</returns>
    public CallbackManager GetChild(string? tag = null)
    {
        var manager = new CallbackManager(parentRunId: RunId);

        manager.SetHandlers(InheritableHandlers);

        manager.AddTags(InheritableTags);
        manager.AddMetadata(InheritableMetadata);

        if (tag != null)
            manager.AddTags(new List<string> { tag }, inherit: false); 

        return manager;
    }
}