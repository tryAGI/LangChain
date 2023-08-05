using LangChain.LLMS;
using LangChain.Schema;

namespace LangChain.Base;

public interface IBaseCallbackHandler
{
    string Name { get; }
    
    public Task HandleLlmStartAsync(
        BaseLlm llm,
        string[] prompts,
        string runId,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null);

    public Task HandleLlmNewTokenAsync(
        string token,
        string runId,
        string? parentRunId = null);

    public Task HandleLlmErrorAsync(
        Exception err,
        string runId,
        string? parentRunId = null);

    public Task HandleLlmEndAsync(
        LlmResult output,
        string runId,
        string? parentRunId = null);

    public Task HandleChatModelStartAsync(
        Dictionary<string, object> llm,
        List<List<object>> messages,
        string runId,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null);

    public Task HandleChainStartAsync(
        Dictionary<string, object> chain,
        Dictionary<string, object> inputs,
        string runId,
        string? parentRunId = null);

    public Task HandleChainErrorAsync(
        Exception err,
        string runId,
        string? parentRunId = null);

    public Task HandleChainEndAsync(
        Dictionary<string, object> outputs,
        string runId,
        string? parentRunId = null);

    public Task HandleToolStartAsync(
        Dictionary<string, object> tool,
        string input,
        string runId,
        string? parentRunId = null);

    public Task HandleToolErrorAsync(
        Exception err,
        string runId,
        string? parentRunId = null);

    public Task HandleToolEndAsync(
        string output,
        string runId,
        string? parentRunId = null);

    public Task HandleTextAsync(
        string text,
        string runId,
        string? parentRunId = null);

    public Task HandleAgentActionAsync(
        Dictionary<string, object> action,
        string runId,
        string? parentRunId = null);

    public Task HandleAgentEndAsync(
        Dictionary<string, object> action,
        string runId,
        string? parentRunId = null);
}