using System.Text.RegularExpressions;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains.ReAct;

/// <inheritdoc/>
public class ReActParserChain : BaseStackableChain
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputText"></param>
    public ReActParserChain(
        string inputKey="text",
        string outputText="answer")
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputText };
    }

    private const string FinalAnswerAction = "Final Answer:";
    private const string MissingActionAfterThoughtErrorMessage = "Invalid Format: Missing 'Action:' after 'Thought:";
    private const string MissingActionInputAfterActionErrorMessage = "Invalid Format: Missing 'Action Input:' after 'Action:'";
    private const string FinalAnswerAndParsableActionErrorMessage = "Parsing LLM output produced both a final answer and a parse-able action:";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="OutputParserException"></exception>
    public object Parse(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        
        bool includesAnswer = text.Contains(FinalAnswerAction);
        string regex = @"Action\s*\d*\s*:[\s]*(.*?)[\s]*Action\s*\d*\s*Input\s*\d*\s*:[\s]*(.*)";
        Match actionMatch = Regex.Match(text, regex, RegexOptions.Singleline);

        if (actionMatch.Success)
        {
            if (includesAnswer)
            {
                Console.WriteLine($"Warning: LLM output contained both a final answer and parseable action. Prioritizing action and continuing. Output: {text}");
                // Continue execution, the parseable action will be returned
            }
            string action = actionMatch.Groups[1].Value.Trim();
            string actionInput = actionMatch.Groups[2].Value.Trim().Trim('\"');

            return new AgentAction(action, actionInput, text);
        }
        else if (includesAnswer)
        {
            return new AgentFinish(text.Split(new[] { FinalAnswerAction }, StringSplitOptions.None)[^1].Trim(), text);
        }

        if (!Regex.IsMatch(text, @"Action\s*\d*\s*:[\s]*(.*?)", RegexOptions.Singleline))
        {
            throw new OutputParserException($"Could not parse LLM output: `{text}`", MissingActionAfterThoughtErrorMessage);
        }
        else if (!Regex.IsMatch(text, @"[\s]*Action\s*\d*\s*Input\s*\d*\s*:[\s]*(.*)", RegexOptions.Singleline))
        {
            throw new OutputParserException($"Could not parse LLM output: `{text}`", MissingActionInputAfterActionErrorMessage);
        }
        else
        {
            throw new OutputParserException($"Could not parse LLM output: `{text}`");
        }
    }

    /// <inheritdoc/>
    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        values.Value[OutputKeys[0]] = Parse(values.Value[InputKeys[0]].ToString()!);
        return Task.FromResult(values);
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="action"></param>
/// <param name="actionInput"></param>
/// <param name="text"></param>
public class AgentAction(string action, string actionInput, string text)
{
    /// <summary>
    /// 
    /// </summary>
    public string Action => action;
    
    /// <summary>
    /// 
    /// </summary>
    public string ActionInput => actionInput;
    
    /// <summary>
    /// 
    /// </summary>
    public string Text => text;

    /// <inheritdoc/>
    public override string ToString()
    {
       return $"Action: {action}, Action Input: {actionInput}";
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="output"></param>
/// <param name="text"></param>
public class AgentFinish(string output, string text)
{
    /// <summary>
    /// 
    /// </summary>
    public string Output => output;
    
    /// <summary>
    /// 
    /// </summary>
    public string Text => text;

    /// <inheritdoc/>
    public override string ToString()
    {
         return $"Final Answer: {output}";
    }
}

