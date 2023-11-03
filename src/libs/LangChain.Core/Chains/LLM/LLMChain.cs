using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Memory;
using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;
using Generation = LangChain.Schema.Generation;

namespace LangChain.Chains.LLM;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LlmChain(LlmChainInput fields) : BaseChain, ILlmChain
{
    public BasePromptTemplate Prompt { get; } = fields.Prompt;
    public IChatModel Llm { get; } = fields.Llm;
    public BaseMemory? Memory { get; } = fields.Memory;
    public string OutputKey { get; set; } = fields.OutputKey;

    public override string ChainType() => "llm_chain";

    public bool? Verbose { get; set; }
    public CallbackManager? CallbackManager { get; set; }

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
    /// <returns>The resulting output <see cref="ChainValues"/>.</returns>
    public override async Task<IChainValues> CallAsync(IChainValues values)
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
        
        if(string.IsNullOrEmpty(OutputKey))
            return new ChainValues(response.Messages.Last().Content);
            
        return new ChainValues(OutputKey,response.Messages.Last().Content);
    }
    
    public async Task<object> Predict(ChainValues values)
    {
        var output = await CallAsync(values);
        return output.Value[OutputKey];
    }

}