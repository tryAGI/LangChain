using LangChain.Base;
using LangChain.Chat;
using LangChain.LLMS;
using LangChain.Schema;

namespace LangChain.Callback;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 
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
    public string Name { get; } = "callback_manager";
    private readonly string? _parentRunId;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentRunId"></param>
    public CallbackManager(string? parentRunId = null)
    {
        Handlers = new List<BaseCallbackHandler>();
        InheritableHandlers = new List<BaseCallbackHandler>();
        _parentRunId = parentRunId;
    }

    public async Task<CallbackManagerForLlmRun> HandleLlmStart(
        BaseLlm llm,
        List<string> prompts,
        string? runId = null,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmStartAsync(llm, prompts.ToArray(), runId ?? Guid.NewGuid().ToString(), _parentRunId, extraParams);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMStart: {ex}");
                }
            }
        }

        return new CallbackManagerForLlmRun(runId, Handlers, InheritableHandlers, _parentRunId);
    }

    public async Task<CallbackManagerForLlmRun> HandleChatModelStart(
        BaseLlm llm,
        List<List<BaseChatMessage>> messages,
        string? runId = null,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        List<string> messageStrings = null;
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                /*try
                {
                    if (handler is IHandleChatModelStart handleChatModelStartHandler)
                    {
                        await handleChatModelStartHandler.HandleChatModelStart(llm, messages, runId ?? Guid.NewGuid().ToString(), _parentRunId, extraParams);
                    }
                    else if (handler is IHandleLLMStart handleLLMStartHandler)
                    {
                        messageStrings = messages.Select(x => GetBufferString(x)).ToList();
                        await handleLLMStartHandler.HandleLLMStart(llm, messageStrings, runId ?? Guid.NewGuid().ToString(), _parentRunId, extraParams);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMStart: {ex}");
                }*/
            }
        }

        return new CallbackManagerForLlmRun(runId, Handlers, InheritableHandlers, _parentRunId);
    }

    public async Task<CallbackManagerForChainRun> HandleChainStart(
        BaseChain chain,
        ChainValues inputs,
        string? runId = null)
    {
        foreach (var handler in Handlers)
        {
            //TODO: Implement methods
            // if (!handler.IgnoreChain)
            // {
            //     try
            //     {
            //         await handler.HandleChainStart(chain, inputs, runId ?? Guid.NewGuid().ToString(), _parentRunId);
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleChainStart: {ex}");
            //     }
            // }
        }

        return new CallbackManagerForChainRun(runId, Handlers, InheritableHandlers, _parentRunId);
    }

    public void AddHandler(BaseCallbackHandler handler, bool inherit = true)
    {
        Handlers.Add(handler);
        if (inherit)
        {
            InheritableHandlers.Add(handler);
        }
    }

    public void AddHandler(BaseCallbackHandler handler)
    {
        throw new NotImplementedException();
    }

    public void RemoveHandler(BaseCallbackHandler handler)
    {
        Handlers.Remove(handler);
        InheritableHandlers.Remove(handler);
    }

    public void SetHandlers(IEnumerable<BaseCallbackHandler> handlers)
    {
        Handlers = handlers.ToList();
    }

    public void SetHandlers(List<BaseCallbackHandler> handlers, bool inherit = true)
    {
        Handlers.Clear();
        InheritableHandlers.Clear();
        foreach (var handler in handlers)
        {
            AddHandler(handler, inherit);
        }
    }

    public CallbackManager Copy(List<BaseCallbackHandler>? additionalHandlers = null, bool inherit = true)
    {
        var manager = new CallbackManager(_parentRunId);
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

    public static CallbackManager FromHandlers(List<Handler> handlers)
    {
        var manager = new CallbackManager();

        foreach (var handler in handlers)
        {
            manager.AddHandler(handler);
        }

        return manager;
    }

    public static async Task<CallbackManager> Configure(
        List<BaseCallbackHandler>? inheritableHandlers = null,
        List<BaseCallbackHandler>? localHandlers = null,
        ICallbackManagerOptions? options = null)
    {
        CallbackManager callbackManager = null;
        if (inheritableHandlers != null || localHandlers != null)
        {
            if (inheritableHandlers is List<BaseCallbackHandler> || inheritableHandlers == null)
            {
                callbackManager = new CallbackManager();
                callbackManager.SetHandlers(inheritableHandlers?.Cast<BaseCallbackHandler>().ToList() ?? new List<BaseCallbackHandler>(), true);
            }

            callbackManager = callbackManager.Copy(
                localHandlers,
                false);
        }
        var verboseEnabled = (Environment.GetEnvironmentVariable("LANGCHAIN_VERBOSE") != null || options?.Verbose == true);
        var tracingV2Enabled = (Environment.GetEnvironmentVariable("LANGCHAIN_TRACING_V2") != null);
        var tracingEnabled = tracingV2Enabled || (Environment.GetEnvironmentVariable("LANGCHAIN_TRACING") != null);
        if (verboseEnabled || tracingEnabled)
        {
            if (callbackManager == null)
            {
                callbackManager = new CallbackManager();
            }
            //TODO: Implement handlers
            /*if (!callbackManager.Handlers.Any(h => h.Name == ConsoleCallbackHandler.Name))
            {
                var consoleHandler = new ConsoleCallbackHandler();
                callbackManager.AddHandler(consoleHandler, true);
            }
            if (!callbackManager.Handlers.Any(h => h.Name == "langchain_tracer"))
            {
                if (tracingV2Enabled)
                {
                    callbackManager.AddHandler(await GetTracingV2CallbackHandler(), true);
                }
                else
                {
                    var session = Environment.GetEnvironmentVariable("LANGCHAIN_SESSION");
                    callbackManager.AddHandler(await GetTracingCallbackHandler(session), true);
                }
            }*/
        }
        return callbackManager;
    }

    private static string GetBufferString(List<BaseChatMessage> messages)
    {
        // Implement your logic here to convert messages to a string
        throw new NotImplementedException();
    }

    public Task HandleLlmStartAsync(Dictionary<string, object> llm, string[] prompts, string runId, string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleLlmNewTokenAsync(string token, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleLlmErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleLlmEndAsync(LlmResult output, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleChatModelStartAsync(Dictionary<string, object> llm, List<List<object>> messages, string runId, string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleChainStartAsync(Dictionary<string, object> chain, Dictionary<string, object> inputs, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleChainErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleChainEndAsync(Dictionary<string, object> outputs, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleToolStartAsync(Dictionary<string, object> tool, string input, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleToolErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleToolEndAsync(string output, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleTextAsync(string text, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleAgentActionAsync(Dictionary<string, object> action, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleAgentEndAsync(Dictionary<string, object> action, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }
}