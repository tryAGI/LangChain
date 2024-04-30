using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Base.Tracers;
using LangChain.LLMS;
using LangChain.Providers;
using LangChain.Retrievers;

namespace LangChain.Callback;

/// <summary>
/// Base callback manager that handles callbacks from LangChain.
/// </summary>
public class CallbackManager
{
    /// <summary>
    /// 
    /// </summary>
    public List<BaseCallbackHandler> Handlers { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public List<BaseCallbackHandler> InheritableHandlers { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; } = "callback_manager";

    /// <summary>
    /// 
    /// </summary>
    public string? ParentRunId { get; }

    /// <summary>
    /// 
    /// </summary>
    protected List<string> Tags { get; }

    /// <summary>
    /// 
    /// </summary>
    protected List<string> InheritableTags { get; }

    /// <summary>
    /// 
    /// </summary>
    protected Dictionary<string, object> Metadata { get; }

    /// <summary>
    /// 
    /// </summary>
    protected Dictionary<string, object> InheritableMetadata { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inheritableMetadata"></param>
    /// <param name="parentRunId"></param>
    /// <param name="handlers"></param>
    /// <param name="inheritableHandlers"></param>
    /// <param name="tags"></param>
    /// <param name="inheritableTags"></param>
    /// <param name="metadata"></param>
    public CallbackManager(
        List<BaseCallbackHandler>? handlers = null,
        List<BaseCallbackHandler>? inheritableHandlers = null,
        List<string>? tags = null,
        List<string>? inheritableTags = null,
        Dictionary<string, object>? metadata = null,
        Dictionary<string, object>? inheritableMetadata = null,
        string? parentRunId = null)
    {
        Handlers = handlers ?? new List<BaseCallbackHandler>();
        InheritableHandlers = inheritableHandlers ?? new List<BaseCallbackHandler>();
        ParentRunId = parentRunId;

        Tags = tags ?? new();
        InheritableTags = inheritableTags ?? new();
        Metadata = metadata ?? new();
        InheritableMetadata = inheritableMetadata ?? new();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="inherit"></param>
    public void AddTags(IReadOnlyList<string> tags, bool inherit = true)
    {
        Tags.RemoveAll(tag => tags.Contains(tag));
        Tags.AddRange(tags);

        if (inherit)
        {
            InheritableTags.AddRange(tags);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tags"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveTags(IReadOnlyList<string> tags)
    {
        tags = tags ?? throw new ArgumentNullException(nameof(tags));

        foreach (var tag in tags)
        {
            Tags.Remove(tag);
            InheritableTags.Remove(tag);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="inherit"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddMetadata(IReadOnlyDictionary<string, object> metadata, bool inherit = true)
    {
        metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

        foreach (var kv in metadata)
        {
            Metadata[kv.Key] = kv.Value;
            if (inherit)
            {
                InheritableMetadata[kv.Key] = kv.Value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveMetadata(IReadOnlyList<string> keys)
    {
        keys = keys ?? throw new ArgumentNullException(nameof(keys));

        foreach (var key in keys)
        {
            Metadata.Remove(key);
            InheritableMetadata.Remove(key);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="prompts"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public async Task<CallbackManagerForLlmRun> HandleLlmStart(
        BaseLlm llm,
        IReadOnlyList<string> prompts,
        string? runId = null,
        string? parentRunId = null,
        IReadOnlyDictionary<string, object>? extraParams = null)
    {
        runId ??= Guid.NewGuid().ToString();

        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmStartAsync(
                        llm,
                        prompts.ToArray(),
                        runId,
                        ParentRunId,
                        extraParams: extraParams).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleLLMStart: {ex}").ConfigureAwait(false);
                }
            }
        }

        return new CallbackManagerForLlmRun(runId, Handlers, InheritableHandlers, ParentRunId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="messages"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public async Task<CallbackManagerForLlmRun> HandleChatModelStart(
        BaseLlm llm,
        IReadOnlyList<List<Message>> messages,
        string? runId = null,
        string? parentRunId = null,
        IReadOnlyDictionary<string, object>? extraParams = null)
    {
        runId ??= Guid.NewGuid().ToString();

        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleChatModelStartAsync(llm, messages, runId, ParentRunId, extraParams).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleLLMStart: {ex}").ConfigureAwait(false);
                }
            }
        }

        return new CallbackManagerForLlmRun(runId, Handlers, InheritableHandlers, ParentRunId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="inputs"></param>
    /// <param name="runId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CallbackManagerForChainRun> HandleChainStart(
        BaseChain chain,
        IChainValues inputs,
        string? runId = null)
    {
        inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
        runId ??= Guid.NewGuid().ToString();

        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreChain)
            {
                try
                {
                    await handler.HandleChainStartAsync(chain, inputs.Value, runId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleChainStart: {ex}").ConfigureAwait(false);
                }
            }
        }

        return new CallbackManagerForChainRun(runId, Handlers, InheritableHandlers, ParentRunId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="retriever"></param>
    /// <param name="query"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public async Task<CallbackManagerForRetrieverRun> HandleRetrieverStart(
        BaseRetriever retriever,
        string query,
        string? runId = null,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        runId ??= Guid.NewGuid().ToString();

        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    // TODO: pass extraParams ?
                    await handler.HandleRetrieverStartAsync(retriever, query, runId, ParentRunId, extraParams: extraParams).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleRetrieverStart: {ex}").ConfigureAwait(false);
                }
            }
        }

        var manager = new CallbackManagerForRetrieverRun(
            runId,
            Handlers,
            InheritableHandlers,
            ParentRunId,
            Tags,
            InheritableTags,
            Metadata,
            InheritableMetadata);

        return manager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="inherit"></param>
    public void AddHandler(BaseCallbackHandler handler, bool inherit = true)
    {
        Handlers.Add(handler);
        if (inherit)
        {
            InheritableHandlers.Add(handler);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    public void RemoveHandler(BaseCallbackHandler handler)
    {
        Handlers.Remove(handler);
        InheritableHandlers.Remove(handler);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlers"></param>
    public void SetHandlers(IEnumerable<BaseCallbackHandler> handlers)
    {
        Handlers = handlers.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="inherit"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetHandlers(List<BaseCallbackHandler> handlers, bool inherit = true)
    {
        handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

        Handlers.Clear();
        InheritableHandlers.Clear();
        foreach (var handler in handlers)
        {
            AddHandler(handler, inherit);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="additionalHandlers"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public CallbackManager Copy(List<BaseCallbackHandler>? additionalHandlers = null, bool inherit = true)
    {
        var manager = new CallbackManager(parentRunId: ParentRunId);
        foreach (var handler in Handlers)
        {
            var inheritable = InheritableHandlers.Contains(handler);
            manager.AddHandler(handler, inheritable);
        }

        if (additionalHandlers != null)
        {
            foreach (var handler in additionalHandlers)
            {
                if (manager.Handlers.Any(h => h.Name == "console_callback_handler" && h.Name == handler.Name))
                {
                    continue;
                }
                manager.AddHandler(handler, inherit);
            }
        }
        return manager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlers"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static CallbackManager FromHandlers(List<Handler> handlers)
    {
        handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

        var manager = new CallbackManager();

        foreach (var handler in handlers)
        {
            manager.AddHandler(handler);
        }

        return manager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inheritableCallbacks"></param>
    /// <param name="localCallbacks"></param>
    /// <param name="verbose"></param>
    /// <param name="localTags"></param>
    /// <param name="inheritableTags"></param>
    /// <param name="localMetadata"></param>
    /// <param name="inheritableMetadata"></param>
    /// <returns></returns>
    // TODO: review! motivation?
    // ICallbackManagerOptions? options = null,
    public static Task<CallbackManager> Configure(ICallbacks? inheritableCallbacks = null,
        ICallbacks? localCallbacks = null,
        bool verbose = false,
        IReadOnlyList<string>? localTags = null,
        IReadOnlyList<string>? inheritableTags = null,
        IReadOnlyDictionary<string, object>? localMetadata = null,
        IReadOnlyDictionary<string, object>? inheritableMetadata = null)
    {
        // TODO: parentRunId using AsyncLocal
        // python version using `contextvars` lib
        //      run_tree = get_run_tree_context()
        //      parent_run_id = None if run_tree is None else getattr(run_tree, "id")
        string? parentId = null;

        CallbackManager callbackManager;

        if (inheritableCallbacks != null || localCallbacks != null)
        {
            switch (inheritableCallbacks)
            {
                case HandlersCallbacks inheritableHandlers:
                    callbackManager = new CallbackManager(parentRunId: parentId);
                    callbackManager.SetHandlers(inheritableHandlers.Value, true);
                    break;

                case ManagerCallbacks managerCallbacks:
                    // ToList() and ToDictionary() used to create copy
                    callbackManager = new CallbackManager(
                        managerCallbacks.Value.Handlers.ToList(),
                        managerCallbacks.Value.InheritableHandlers.ToList(),
                        managerCallbacks.Value.Tags.ToList(),
                        managerCallbacks.Value.InheritableTags.ToList(),
                        managerCallbacks.Value.Metadata.ToDictionary(kv => kv.Key, kv => kv.Value),
                        managerCallbacks.Value.InheritableMetadata.ToDictionary(kv => kv.Key, kv => kv.Value),
                        parentRunId: managerCallbacks.Value.ParentRunId);
                    break;

                default:
                    callbackManager = new CallbackManager(parentRunId: parentId);
                    break;
            }

            var localHandlers = localCallbacks switch
            {
                HandlersCallbacks localHandlersCallbacks => localHandlersCallbacks.Value,
                ManagerCallbacks managerCallbacks => managerCallbacks.Value.Handlers,
                _ => new List<BaseCallbackHandler>()
            };

            callbackManager = callbackManager.Copy(localHandlers, false);
        }
        else
        {
            callbackManager = new CallbackManager(parentRunId: parentId);
        }

        if (inheritableTags != null) callbackManager.AddTags(inheritableTags);
        if (localTags != null) callbackManager.AddTags(localTags, inherit: false);

        if (inheritableMetadata != null) callbackManager.AddMetadata(inheritableMetadata);
        if (localMetadata != null) callbackManager.AddMetadata(localMetadata, inherit: false);

        var verboseEnabled = (Environment.GetEnvironmentVariable("LANGCHAIN_VERBOSE") != null || verbose);
        var tracingV2Enabled = (Environment.GetEnvironmentVariable("LANGCHAIN_TRACING_V2") != null);
        var tracingEnabled = tracingV2Enabled || (Environment.GetEnvironmentVariable("LANGCHAIN_TRACING") != null);
        if (verboseEnabled || tracingEnabled)
        {
            // TODO: replace inlined name "console_callback_handler" with const
            if (callbackManager.Handlers.All(h => h.Name != "console_callback_handler"))
            {
                var consoleHandler = new ConsoleCallbackHandler();
                callbackManager.AddHandler(consoleHandler, inherit: true);
            }

            // TODO: implement handlers
            // if (callbackManager.Handlers.All(h => h.Name != "langchain_tracer"))
            // {
            //     if (tracingV2Enabled)
            //     {
            //         callbackManager.AddHandler(await GetTracingV2CallbackHandler(), true);
            //     }
            //     else
            //     {
            //         var session = Environment.GetEnvironmentVariable("LANGCHAIN_SESSION");
            //         callbackManager.AddHandler(await GetTracingCallbackHandler(session), true);
            //     }
            // }
        }

        return Task.FromResult(callbackManager);
    }
}