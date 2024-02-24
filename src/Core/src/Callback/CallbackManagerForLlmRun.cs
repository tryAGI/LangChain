using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

/// <inheritdoc/>
public class CallbackManagerForLlmRun : BaseRunManager
{
    /// <inheritdoc/>
    public CallbackManagerForLlmRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    // TODO: remove?
    public async Task HandleLlmNewTokenAsync(string token, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmNewTokenAsync(token, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleLLMNewToken: {ex}").ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    public async Task HandleLlmErrorAsync(Exception error, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmErrorAsync(error, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleLLMError: {ex}").ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    public async Task HandleLlmEndAsync(LlmResult output, string runId, string parentRunId)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleLlmEndAsync(output, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleLLMEnd: {ex}").ConfigureAwait(false);
                }
            }
        }
    }
}