using LangChain.NET.Base;
using LangChain.NET.Schema;

namespace LangChain.NET.Callback;

public class CallbackManagerForChainRun : BaseRunManager
{
    public CallbackManagerForChainRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public CallbackManager GetChild()
    {
        var manager = new CallbackManager(RunId);
        manager.SetHandlers(InheritableHandlers);
        return manager;
    }

    public async Task HandleChainEndAsync(ChainValues output)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreChain)
            {
                try
                {
                    await handler.HandleChainEndAsync(output.Value, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleChainEnd: {ex}");
                }
            }
        }
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

    public async Task HandleChainErrorAsync(Exception error, string runId, string? parentRunId = null)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleChainErrorAsync(error, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleChainError: {ex}");
                }
            }
        }
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