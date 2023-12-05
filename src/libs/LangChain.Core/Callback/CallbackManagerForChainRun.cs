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

    public async Task HandleChainEndAsync(IChainValues input, IChainValues output)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        output = output ?? throw new ArgumentNullException(nameof(output));
        
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreChain)
            {
                try
                {
                    await handler.HandleChainEndAsync(
                        input.Value,
                        output.Value,
                        RunId,
                        ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleChainEnd: {ex}").ConfigureAwait(false);
                }
            }
        }
    }

    public async Task HandleChainErrorAsync(Exception error, IChainValues input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleChainErrorAsync(error, RunId, input.Value, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleChainError: {ex}").ConfigureAwait(false);
                }
            }
        }
    }

    public async Task HandleTextAsync(string text)
    {
        foreach (var handler in Handlers)
        {
            if (!handler.IgnoreLlm)
            {
                try
                {
                    await handler.HandleTextAsync(text, RunId, ParentRunId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Error in handler {handler.GetType().Name}, HandleText: {ex}").ConfigureAwait(false);
                }
            }
        }
    }
}