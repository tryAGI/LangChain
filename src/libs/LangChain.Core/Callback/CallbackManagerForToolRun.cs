using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

public class CallbackManagerForToolRun : BaseRunManager
{
    public CallbackManagerForToolRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public CallbackManager GetChild()
    {
        var manager = new CallbackManager(RunId);
        manager.SetHandlers(InheritableHandlers);
        return manager;
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