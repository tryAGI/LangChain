using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Common;
using LangChain.Memory;
using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;
using Generation = LangChain.Schema.Generation;

namespace LangChain.Chains.LLM;

public class LlmChain(LlmChainInput fields) : BaseChain(fields), ILlmChain
{
    public BasePromptTemplate Prompt { get; } = fields.Prompt;
    public IChatModel Llm { get; } = fields.Llm;
    public BaseMemory? Memory { get; } = fields.Memory;
    public string OutputKey { get; set; } = fields.OutputKey;
    public bool ReturnFinalOnly { get; set; } = fields.ReturnFinalOnly;

    public BaseLlmOutputParser<string> _outputParser { get; set; } = new StrOutputParser();

    public override string ChainType() => "llm_chain";

    public CallbackManager? CallbackManager { get; set; }

    public bool Verbose { get; set; }
    public ICallbacks? Callbacks { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, object> Metadata { get; set; }

    public override string[] InputKeys => Prompt.InputVariables.ToArray();
    public override string[] OutputKeys => new[] { OutputKey };

    protected async Task<object?> GetFinalOutput(
        List<Generation> generations,
        BasePromptValue promptValue,
        CallbackManagerForChainRun? runManager = null)
    {
        return generations[0].Text;
    }

    /// <summary>
    /// Execute the chain.
    /// </summary>
    /// <param name="values">The values to use when executing the chain.</param>
    /// <param name="runManager"></param>
    /// <returns>The resulting output <see cref="ChainValues"/>.</returns>
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager)
    {
        List<string>? stop = new List<string>();

        if (values.Value.TryGetValue("stop", out var value))
        {
            var stopList = value as List<string>;

            stop = stopList;
        }

        var promptValue = await Prompt.FormatPromptValue(new InputValues(values.Value));
        var chatMessages = promptValue.ToChatMessages().WithHistory(Memory);
        if (Verbose == true)
        {
            Console.WriteLine(string.Join("\n\n", chatMessages));
            Console.WriteLine("\n".PadLeft(Console.WindowWidth, '>'));
        }

        var response = await Llm.GenerateAsync(new ChatRequest(chatMessages, stop));
        if (Verbose == true)
        {
            Console.WriteLine(string.Join("\n\n", response.Messages.Except(chatMessages)));
            Console.WriteLine("\n".PadLeft(Console.WindowWidth, '<'));
        }

        var returnDict = new Dictionary<string, object>();

        var outputKey = string.IsNullOrEmpty(OutputKey) ? "text" : OutputKey;
        returnDict[outputKey] = response.Messages.Last().Content;

        values.Value.TryAddKeyValues(returnDict);

        return values;
    }

    /// <summary>
    /// Call the chain on all inputs in the list.
    /// </summary>
    public override async Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs)
    {
        var callbackManager = await CallbackManager.Configure(inheritableCallbacks: null, localCallbacks: Callbacks, verbose: Verbose);
        var runManager = await callbackManager.HandleChainStart(this, new ChainValues("input_list", inputs));

        LlmResult response;
        try
        {
            response = await GenerateAsync(inputs, runManager);
        }
        catch (Exception exception)
        {
            await runManager.HandleChainErrorAsync(exception, new ChainValues("inputs", inputs));
            throw;
        }

        var outputs = CreateOutputs(response);
        await runManager.HandleChainEndAsync(new ChainValues("inputs", inputs), new ChainValues("outputs", outputs));

        return outputs;
    }

    private async Task<LlmResult> GenerateAsync(IReadOnlyList<ChainValues> inputs, CallbackManagerForChainRun runManager)
    {
        var (prompts, stop) = await PreparePromptsAsync(inputs, runManager);

        var responseTasks = new List<Task<ChatResponse>>();
        foreach (var prompt in prompts)
        {
            var request = new ChatRequest(prompt.ToChatMessages(), stop);
            responseTasks.Add(Llm.GenerateAsync(request));
        }

        var responses = await Task.WhenAll(responseTasks);

        var generations = responses.Select(response =>
                new Generation[]
                {
                    new Generation
                    {
                        Text = response.Messages.Last().Content
                    }
                })
            .ToArray();

        var result = new LlmResult
        {
            Generations = generations
        };

        return result;
    }

    /// <summary>
    /// Prepare prompts from inputs.
    /// </summary>
    /// <param name="inputList"></param>
    /// <param name="runManager"></param>
    /// <returns></returns>
    private async Task<(List<BasePromptValue>, List<string>?)> PreparePromptsAsync(
        IReadOnlyList<ChainValues> inputList,
        CallbackManagerForChainRun runManager = null)
    {
        List<string>? stop = null;
        if (inputList.Count == 0)
        {
            return (new List<BasePromptValue>(), stop);
        }

        if (inputList[0].Value.ContainsKey("stop"))
        {
            stop = inputList[0].Value["stop"] as List<string>;
        }

        var prompts = new List<BasePromptValue>();

        foreach (var inputs in inputList)
        {
            var selectedInputs = Prompt.InputVariables.ToDictionary(v => v, v => inputs.Value[v]);
            var prompt = await Prompt.FormatPromptValue(new InputValues(selectedInputs));

            if (runManager != null)
            {
                var text = "Prompt after formatting:\n" + prompt;
                await runManager.HandleTextAsync(text);
            }

            if (inputs.Value.ContainsKey("stop") && inputs.Value["stop"] != stop)
            {
                throw new ArgumentException("If `stop` is present in any inputs, should be present in all.");
            }

            prompts.Add(prompt);
        }

        return (prompts, stop);
    }

    /// <summary> Create outputs from response. </summary>
    private List<IChainValues> CreateOutputs(LlmResult llmResult)
    {
        // Get the text of the top generated string.
        var result = llmResult.Generations
            .Select(generation =>
            {
                var dictionary = new Dictionary<string, object>
                {
                    [OutputKey] = _outputParser.ParseResult(generation)
                };

                if (!ReturnFinalOnly)
                {
                    dictionary["full_generation"] = generation;
                }

                return new ChainValues(dictionary);
            })
            .Cast<IChainValues>()
            .ToList();

        return result;
    }

    public async Task<object> Predict(ChainValues values)
    {
        var output = await CallAsync(values);
        return output.Value[OutputKey];
    }
}