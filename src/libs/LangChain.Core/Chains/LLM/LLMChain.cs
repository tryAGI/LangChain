using System.Threading.Tasks;
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

/// <summary>
/// 
/// </summary>
/// <param name="fields"></param>
public class LlmChain(LlmChainInput fields) : BaseChain(fields), ILlmChain
{
    /// <summary>
    /// 
    /// </summary>
    public BasePromptTemplate Prompt { get; } = fields.Prompt;
    
    /// <summary>
    /// 
    /// </summary>
    public IChatModel Llm { get; } = fields.Llm;
    
    /// <summary>
    /// 
    /// </summary>
    public BaseMemory? Memory { get; } = fields.Memory;
    
    /// <summary>
    /// 
    /// </summary>
    public string OutputKey { get; set; } = fields.OutputKey;
    
    /// <summary>
    /// 
    /// </summary>
    public bool ReturnFinalOnly { get; set; } = fields.ReturnFinalOnly;

    /// <summary>
    /// 
    /// </summary>
    public BaseLlmOutputParser<string> OutputParser { get; set; } = new StrOutputParser();

    /// <inheritdoc/>
    public override string ChainType() => "llm_chain";

    /// <summary>
    /// 
    /// </summary>
    public CallbackManager? CallbackManager { get; set; }

    /// <inheritdoc/>
    public bool Verbose { get; set; }
    
    /// <inheritdoc/>
    public ICallbacks? Callbacks { get; set; }
    
    /// <inheritdoc/>
    public List<string> Tags { get; set; } = new();
    
    /// <inheritdoc/>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <inheritdoc/>
    public override IReadOnlyList<string> InputKeys => Prompt.InputVariables.ToArray();
    
    /// <inheritdoc/>
    public override IReadOnlyList<string> OutputKeys => new[] { OutputKey };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generations"></param>
    /// <param name="promptValue"></param>
    /// <param name="runManager"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected Task<object?> GetFinalOutput(
        List<Generation> generations,
        BasePromptValue promptValue,
        CallbackManagerForChainRun? runManager = null)
    {
        generations = generations ?? throw new ArgumentNullException(nameof(generations));
        
        return Task.FromResult<object?>(generations[0].Text);
    }

    /// <summary>
    /// Execute the chain.
    /// </summary>
    /// <param name="values">The values to use when executing the chain.</param>
    /// <param name="runManager"></param>
    /// <returns>The resulting output <see cref="ChainValues"/>.</returns>
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        List<string> stop;
        if (values.Value.TryGetValue("stop", out var value) && value is IEnumerable<string> stopList)
        {
            stop = stopList.ToList();
        }
        else
        {
            stop = new List<string>();
        }

        var promptValue = await Prompt.FormatPromptValue(new InputValues(values.Value)).ConfigureAwait(false);
        var chatMessages = promptValue.ToChatMessages().WithHistory(Memory);
        if (Verbose)
        {
            Console.WriteLine(string.Join("\n\n", chatMessages));
            Console.WriteLine("\n".PadLeft(Console.WindowWidth, '>'));
        }

        var response = await Llm.GenerateAsync(new ChatRequest(chatMessages, stop)).ConfigureAwait(false);
        if (Verbose)
        {
            Console.WriteLine(string.Join("\n\n", response.Messages.Except(chatMessages)));
            Console.WriteLine("\n".PadLeft(Console.WindowWidth, '<'));
        }

        var returnDict = new Dictionary<string, object>();

        var outputKey = string.IsNullOrEmpty(OutputKey) ? "text" : OutputKey;
        returnDict[outputKey] = response.Messages.Last().Content;

        returnDict.TryAddKeyValues(values.Value);

        return new ChainValues(returnDict);
    }

    /// <summary>
    /// Call the chain on all inputs in the list.
    /// </summary>
    public override async Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs)
    {
        var callbackManager = await CallbackManager.Configure(inheritableCallbacks: null, localCallbacks: Callbacks, verbose: Verbose).ConfigureAwait(false);
        var runManager = await callbackManager.HandleChainStart(this, new ChainValues("input_list", inputs)).ConfigureAwait(false);

        LlmResult response;
        try
        {
            response = await GenerateAsync(inputs, runManager).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await runManager.HandleChainErrorAsync(exception, new ChainValues("inputs", inputs)).ConfigureAwait(false);
            throw;
        }

        var outputs = await CreateOutputs(response).ConfigureAwait(false);
        await runManager.HandleChainEndAsync(new ChainValues("inputs", inputs), new ChainValues("outputs", outputs)).ConfigureAwait(false);

        return outputs;
    }

    private async Task<LlmResult> GenerateAsync(IReadOnlyList<ChainValues> inputs, CallbackManagerForChainRun runManager)
    {
        var (prompts, stop) = await PreparePromptsAsync(inputs, runManager).ConfigureAwait(false);

        var responseTasks = new List<Task<ChatResponse>>();
        foreach (var prompt in prompts)
        {
            var request = new ChatRequest(prompt.ToChatMessages(), stop);
            responseTasks.Add(Llm.GenerateAsync(request));
        }

        var responses = await Task.WhenAll(responseTasks).ConfigureAwait(false);

        var generations = responses.Select(response =>
                new Generation[]
                {
                    new()
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
        CallbackManagerForChainRun? runManager = null)
    {
        List<string>? stop = null;
        if (inputList.Count == 0)
        {
            return (new List<BasePromptValue>(), stop);
        }

        if (inputList[0].Value.TryGetValue("stop", out var value))
        {
            stop = value as List<string>;
        }

        var prompts = new List<BasePromptValue>();

        foreach (var inputs in inputList)
        {
            var selectedInputs = Prompt.InputVariables.ToDictionary(v => v, v => inputs.Value[v]);
            var prompt = await Prompt.FormatPromptValue(new InputValues(selectedInputs)).ConfigureAwait(false);

            if (runManager != null)
            {
                var text = "Prompt after formatting:\n" + prompt;
                await runManager.HandleTextAsync(text).ConfigureAwait(false);
            }

            if (inputs.Value.TryGetValue("stop", out var result) && result != stop)
            {
                throw new ArgumentException("If `stop` is present in any inputs, should be present in all.");
            }

            prompts.Add(prompt);
        }

        return (prompts, stop);
    }

    /// <summary> Create outputs from response. </summary>
    private async Task<List<IChainValues>> CreateOutputs(LlmResult llmResult)
    {
        var chainValues = new List<IChainValues>();
        // Get the text of the top generated string.
        foreach (var generation in llmResult.Generations)
        {
            var dictionary = new Dictionary<string, object>
            {
                [OutputKey] = await OutputParser.ParseResult(generation).ConfigureAwait(false)
            };

            if (!ReturnFinalOnly)
            {
                dictionary["full_generation"] = generation;
            }

            chainValues.Add(new ChainValues(dictionary));
        }

        return chainValues;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public async Task<object> Predict(ChainValues values)
    {
        var output = await CallAsync(values).ConfigureAwait(false);
        return output.Value[OutputKey];
    }
}