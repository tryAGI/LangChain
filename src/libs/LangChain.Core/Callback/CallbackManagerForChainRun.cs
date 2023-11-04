using LangChain.Abstractions.Schema;
using LangChain.Base;

namespace LangChain.Callback;

public class CallbackManagerForChainRun : ParentRunManager, IRunManagerImplementation<CallbackManagerForChainRun>
{
    public CallbackManagerForChainRun()
    {
        
    }

    public CallbackManagerForChainRun(
        string runId,
        List<BaseCallbackHandler> handlers,
        List<BaseCallbackHandler> inheritableHandlers,
        string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public async Task HandleChainEndAsync(IChainValues output)
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

    public async Task HandleChainErrorAsync(Exception error)
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

    public Task HandleTextAsync(string text, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }
}