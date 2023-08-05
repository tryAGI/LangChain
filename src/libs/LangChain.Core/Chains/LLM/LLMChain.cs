using LangChain.NET.Base;
using LangChain.NET.Callback;
using LangChain.NET.Prompts.Base;
using LangChain.NET.Schema;

namespace LangChain.NET.Chains.LLM;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LlmChain : BaseChain, ILlmChainInput
{
    public BasePromptTemplate Prompt { get; }
    public BaseLanguageModel Llm { get; }
    public string OutputKey { get; set; }
    public override string ChainType() => "llm_chain";

    public bool? Verbose { get; set; }
    public CallbackManager? CallbackManager { get; set; }

    public override string[] InputKeys => Prompt.InputVariables.ToArray();
    public override string[] OutputKeys => new[] { OutputKey };

    public LlmChain(LlmChainInput fields)
    {
        Prompt = fields.Prompt;
        Llm = fields.Llm;
        OutputKey = fields.OutputKey;
    }

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
    public override async Task<ChainValues> Call(ChainValues values)
    {
        List<string>? stop = new List<string>();

        if (values.Value.TryGetValue("stop", out var value))
        {
            var stopList = value as List<string>;
                
            stop = stopList;
        }
        
        BasePromptValue promptValue = await Prompt.FormatPromptValue(new InputValues(values.Value));
        var generationResult = await Llm.GeneratePrompt(new List<BasePromptValue> { promptValue }.ToArray(), stop);
        var generations = generationResult.Generations;
        
        return new ChainValues(await GetFinalOutput(generations.ToList(), promptValue));
    }
    
    public async Task<object> Predict(ChainValues values)
    {
        var output = await Call(values);
        return output.Value[OutputKey];
    }
}