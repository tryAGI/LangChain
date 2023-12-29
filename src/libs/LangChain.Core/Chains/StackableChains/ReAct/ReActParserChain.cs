using System.Text.RegularExpressions;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains.ReAct;

public class ReActParserChain : BaseStackableChain
{



    public ReActParserChain(string inputKey="text",string outputText="answer")
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputText };
    }

    private const string FinalAnswerAction = "Final Answer:";
    private const string MissingActionAfterThoughtErrorMessage = "Invalid Format: Missing 'Action:' after 'Thought:";
    private const string MissingActionInputAfterActionErrorMessage = "Invalid Format: Missing 'Action Input:' after 'Action:'";
    private const string FinalAnswerAndParsableActionErrorMessage = "Parsing LLM output produced both a final answer and a parse-able action:";



    public object Parse(string text)
    {
        bool includesAnswer = text.Contains(FinalAnswerAction);
        string regex = @"Action\s*\d*\s*:[\s]*(.*?)[\s]*Action\s*\d*\s*Input\s*\d*\s*:[\s]*(.*)";
        Match actionMatch = Regex.Match(text, regex, RegexOptions.Singleline);

        if (actionMatch.Success)
        {
            if (includesAnswer)
            {
                throw new OutputParserException($"{FinalAnswerAndParsableActionErrorMessage}: {text}");
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

    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values.Value[this.OutputKeys[0]] = Parse(values.Value[this.InputKeys[0]].ToString());
        return Task.FromResult(values);
    }
}

public class AgentAction(string action, string actionInput, string text)
{
   public string Action => action;
   public string ActionInput => actionInput;
   public string Text => text;

   public override string ToString()
   {
       return $"Action: {action}, Action Input: {actionInput}";
   }
}

public class AgentFinish(string output, string text)
{
   public string Output => output;
   public string Text => text;

   public override string ToString()
   {
         return $"Final Answer: {output}";
    }
}

