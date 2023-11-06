namespace LangChain.Base.Tracers;

public class FunctionCallbackHandlerInput : BaseCallbackHandlerInput
{
    public Func<string, Task> Function { get; set; }
}

public class FunctionCallbackHandler(FunctionCallbackHandlerInput input) : BaseTracer(input)
{
    public override string Name => "function_callback_handler";
    protected override Task PersistRun(Run run) => Task.CompletedTask;

    public Func<string, Task> Function { get; } = input.Function;

    protected override void OnRunCreate(Run run)
    {
        throw new NotImplementedException();
    }

    protected override void OnRunUpdate(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleLlmStartAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleLlmNewTokenAsync(Run run, string token)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleLlmErrorAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleLlmEndAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChatModelStartAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChainStartAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChainErrorAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChainEndAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleToolStartAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleToolErrorAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleToolEndAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleTextAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleAgentActionAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleAgentEndAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleRetrieverStartAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleRetrieverEndAsync(Run run)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleRetrieverErrorAsync(Run run)
    {
        throw new NotImplementedException();
    }
}