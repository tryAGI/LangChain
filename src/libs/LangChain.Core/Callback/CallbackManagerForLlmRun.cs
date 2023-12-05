using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

public class CallbackManagerForLlmRun : BaseRunManager
{
    public CallbackManagerForLlmRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

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
                    await handler.HandleLlmErrorAsync(error, RunId, ParentRunId).ConfigureAwait(false);
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
                    await handler.HandleLlmEndAsync(output, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in handler {handler.GetType().Name}, HandleLLMEnd: {ex}");
                }
            }
        }
    }
}