using LangChain.Abstractions.Chains.Base;
using LangChain.Docstore;
using LangChain.LLMS;
using LangChain.Providers;
using LangChain.Retrievers;
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
    public abstract Task HandleLlmStartAsync(BaseLlm llm, string[] prompts, string runId, string? parentRunId = null,
        IReadOnlyList<string>? tags = null, IReadOnlyDictionary<string, object>? metadata = null,
        string? name = null, IReadOnlyDictionary<string, object>? extraParams = null);

    /// <summary>
    /// Run on new LLM token. Only available when streaming is enabled.
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="messages"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public Task HandleChatModelStartAsync(BaseLlm llm,
        IReadOnlyList<List<Message>> messages,
        string runId,
        string? parentRunId = null,
        IReadOnlyDictionary<string, object>? extraParams = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="inputs"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <param name="runType"></param>
    /// <param name="name"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public Task HandleChainStartAsync(IChain chain,
        Dictionary<string, object> inputs,
        string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="err"></param>
    /// <param name="runId"></param>
    /// <param name="inputs"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleChainErrorAsync(
        Exception err,
        string runId,
        Dictionary<string, object> inputs,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="outputs"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleChainEndAsync(
        Dictionary<string, object>? inputs,
        Dictionary<string, object> outputs,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tool"></param>
    /// <param name="input"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <param name="runType"></param>
    /// <param name="name"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public Task HandleToolStartAsync(
        Dictionary<string, object> tool,
        string input,
        string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="err"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleToolErrorAsync(
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
    public Task HandleToolEndAsync(
        string output,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleTextAsync(
        string text,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleAgentActionAsync(
        Dictionary<string, object> action,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleAgentEndAsync(
        Dictionary<string, object> action,
        string runId,
        string? parentRunId = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="retriever"></param>
    /// <param name="query"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <param name="runType"></param>
    /// <param name="name"></param>
    /// <param name="extraParams"></param>
    /// <returns></returns>
    public Task HandleRetrieverStartAsync(
        BaseRetriever retriever,
        string query,
        string runId,
        string? parentRunId,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="documents"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleRetrieverEndAsync(
        string query,
        List<Document> documents,
        string runId,
        string? parentRunId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="query"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    public Task HandleRetrieverErrorAsync(
        Exception exception,
        string query,
        string runId,
        string? parentRunId);
}