namespace LangChain.Base.Tracers;

/// <inheritdoc />
public class ConsoleCallbackHandlerInput : BaseCallbackHandlerInput
{
}

/// <inheritdoc />
public class ConsoleCallbackHandler(ConsoleCallbackHandlerInput fields) : BaseTracer(fields)
{
    /// <inheritdoc />
    public ConsoleCallbackHandler() : this(new ConsoleCallbackHandlerInput())
    {

    }

    /// <inheritdoc />
    public override string Name => "console_callback_handler";
    
    /// <inheritdoc />
    protected override Task PersistRun(Run run) => Task.CompletedTask;

    /// <inheritdoc />
    protected override async Task HandleLlmStartAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        object inputs = run.Inputs.TryGetValue("prompts", out var input)
            ? new Dictionary<string, List<string>> { { "prompts", (input as List<string>)?.Select(p => p.Trim()).ToList() } }
            : run.Inputs;

        Print(
            $"{GetColoredText("[llm/start]", ConsoleFormats.Green)} {GetColoredText($"[{crumbs}] Entering LLM run with input:", ConsoleFormats.Bold)}\n" +
            $"{JsonSerializeOrDefault(inputs, "[inputs]")}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleLlmNewTokenAsync(Run run, string token) { }

    /// <inheritdoc />
    protected override async Task HandleLlmErrorAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);

        Print($"{GetColoredText("[llm/error]", ConsoleFormats.Red)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] LLM run errored with error:", ConsoleFormats.Bold)}\n" +
              $"{JsonSerializeOrDefault(run.Error, "[error]")}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleLlmEndAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);

        Print($"{GetColoredText("[llm/end]", ConsoleFormats.Blue)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] Exiting LLM run with output:", ConsoleFormats.Bold)}\n" +
              $"{JsonSerializeOrDefault(run.Outputs, "[response]")}"
            );
    }

    /// <inheritdoc />
    protected override async Task HandleChatModelStartAsync(Run run) { }

    /// <inheritdoc />
    protected override async Task HandleChainStartAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        var runType = run.RunType.Capitalize();
        var input = JsonSerializeOrDefault(run.Inputs, "[inputs]");

        Print(
            $"{GetColoredText("[chain/start]", ConsoleFormats.Green)} {GetColoredText($"[{crumbs}] Entering {runType} run with input:", ConsoleFormats.Bold)}\n" +
            $"{input}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleChainErrorAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        var runType = run.RunType.Capitalize();
        var error = JsonSerializeOrDefault(run.Error, "[error]");
        Print(
            $"{GetColoredText("[chain/error]", ConsoleFormats.Red)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] {runType} run errored with error:", ConsoleFormats.Bold)}\n" +
            $"{error}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleChainEndAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        var runType = run.RunType.Capitalize();
        var outputs = JsonSerializeOrDefault(run.Outputs, "[outputs]");

        Print(
            $"{GetColoredText("[chain/end]", ConsoleFormats.Blue)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] Exiting {runType} run with output:", ConsoleFormats.Bold)}\n" +
            $"{outputs}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleToolStartAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        Print(
            $"{GetColoredText("[chain/start]", ConsoleFormats.Green)} {GetColoredText($"[{crumbs}] Entering Tool run with input:", ConsoleFormats.Bold)}\n" +
            $"{run.Inputs["input"].ToString()?.Trim()}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleToolErrorAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        Print(
            $"{GetColoredText("[chain/error]", ConsoleFormats.Red)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] Tool run errored with error:", ConsoleFormats.Bold)}\n" +
            $"{run.Error}"
        );
    }

    /// <inheritdoc />
    protected override async Task HandleToolEndAsync(Run run)
    {
        run = run ?? throw new ArgumentNullException(nameof(run));
        
        var crumbs = GetBreadcrumbs(run);
        if (run.Outputs.Count != 0)
            Print(
                $"{GetColoredText("[chain/end]", ConsoleFormats.Blue)} {GetColoredText($"[{crumbs}] [{Elapsed(run)}] Exiting Tool run with output:", ConsoleFormats.Bold)}\n" +
                $"{run.Outputs["output"].ToString()?.Trim()}"
            );
    }

    /// <inheritdoc />
    protected override async Task HandleTextAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override async Task HandleAgentActionAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override async Task HandleAgentEndAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override async Task HandleRetrieverStartAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override async Task HandleRetrieverEndAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override async Task HandleRetrieverErrorAsync(Run run)
    {
    }

    /// <inheritdoc />
    protected override void OnRunCreate(Run run)
    {
    }

    /// <inheritdoc />
    protected override void OnRunUpdate(Run run)
    {
    }

    private List<Run> GetParents(Run run)
    {
        var parents = new List<Run>();
        var currentRun = run;
        while (currentRun.ParentRunId != null)
        {
            if (RunMap.TryGetValue(currentRun.ParentRunId, out var parent) && parent != null)
            {
                parents.Add(parent);
                currentRun = parent;
            }
            else break;
        }

        return parents;
    }

    private string GetBreadcrumbs(Run run)
    {
        var parents = GetParents(run);
        parents.Reverse();
        parents.Add(run);

        var breadcrumbs = parents.Select((parent, i) => $"{parent.ExecutionOrder}:{parent.RunType}:{parent.Name}");
        var result = string.Join(" > ", breadcrumbs);

        return result;
    }

    private void Print(string text)
    {
        Console.WriteLine(text);
    }

    private string GetColoredText(string text, string format)
    {
        return $"{format}{text}{ConsoleFormats.Normal}";
    }

    private string JsonSerializeOrDefault(object obj, string @default)
    {
        try
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
        catch (Exception)
        {
            return @default;
        }
    }

    /// <summary>
    /// Get the elapsed time of a run.
    /// </summary>
    /// <returns>A string with the elapsed time in seconds or milliseconds if time is less than a second.</returns>
    private string Elapsed(Run run)
    {
        if (!run.EndTime.HasValue)
            return "N/A";

        var elapsedTime = run.EndTime.Value - run.StartTime;
        var milliseconds = elapsedTime.TotalMilliseconds;

        return elapsedTime.TotalMilliseconds < 1000
            ? $"{milliseconds}ms"
            : $"{elapsedTime.TotalSeconds:F1}s";
    }

    private static class ConsoleFormats
    {
        public static string Normal = Console.IsOutputRedirected ? "" : "\x1b[39m";
        public static string Red = Console.IsOutputRedirected ? "" : "\x1b[91m";
        public static string Green = Console.IsOutputRedirected ? "" : "\x1b[92m";
        public static string Yellow = Console.IsOutputRedirected ? "" : "\x1b[93m";
        public static string Blue = Console.IsOutputRedirected ? "" : "\x1b[94m";
        public static string Bold = Console.IsOutputRedirected ? "" : "\x1b[1m";
        public static string Underline = Console.IsOutputRedirected ? "" : "\x1b[4m";
    }
}