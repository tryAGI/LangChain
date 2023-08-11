using LangChain.LLMS;
using LangChain.Schema;

namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public interface IBaseCallbackHandler
{
    /// <summary>
    /// 
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="prompts"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public Task HandleLlmStartAsync(
        BaseLlm llm,
        string[] prompts,
        string runId,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleLlmNewTokenAsync(
        string token,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="err"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleLlmErrorAsync(
        Exception err,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
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