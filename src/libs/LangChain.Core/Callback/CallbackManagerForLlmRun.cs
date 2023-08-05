using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

public class CallbackManagerForLlmRun : BaseRunManager
{
    public CallbackManagerForLlmRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public Task HandleToolEndAsync(string output, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleTextAsync(string text, string runId, string parentRunId)
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

    public Task HandleLlmStartAsync(Dictionary<string, object> llm, string[] prompts, string runId, string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        throw new NotImplementedException();
    }

    public async Task HandleLlmNewTokenAsync(string token, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmNewTokenAsync(token, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMNewToken: {ex}");
                }
            }
        }
    }

    public async Task HandleLlmErrorAsync(Exception error, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmErrorAsync(error, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMError: {ex}");
                }
            }
        }
    }

    public async Task HandleLlmEndAsync(LlmResult output, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmEndAsync(output, RunId, ParentRunId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMEnd: {ex}");
                }
            }
        }
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
}