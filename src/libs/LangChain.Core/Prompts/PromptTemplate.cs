using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PromptTemplate : BaseStringPromptTemplate
{
    public string Template { get; set; }
    public TemplateFormatOptions? TemplateFormat { get; set; } = TemplateFormatOptions.FString;
    public bool? ValidateTemplate { get; set; } = true;
    
    public new Dictionary<string, object> PartialVariables { get; set; } = new();

    public PromptTemplate(IPromptTemplateInput input)
        : base(input)
    {
        Template = input.Template;
        TemplateFormat = input.TemplateFormat ?? TemplateFormatOptions.FString;
        ValidateTemplate = input.ValidateTemplate ?? true;
        PartialVariables = input.PartialVariables;

        if (ValidateTemplate.Value)
        {
            var totalInputVariables = InputVariables.ToList();
            if (PartialVariables != null)
            {
                totalInputVariables.AddRange(PartialVariables.Keys);
            }

            CheckValidTemplate(Template, TemplateFormat.Value, totalInputVariables);
        }
    }

    protected override string GetPromptType()
    {
        return "prompt";
    }

    public override async Task<string> Format(InputValues values)
    {
        InputValues allValues = await MergePartialAndUserVariables(values);
        return RenderTemplate(Template, TemplateFormat.Value, allValues.Value);
    }

    public static PromptTemplate FromExamples(
        IEnumerable<string> examples,
        string suffix,
        IEnumerable<string> inputVariables,
        string exampleSeparator = "\n\n",
        string prefix = "")
    {
        var template = $"{prefix}\n{string.Join(exampleSeparator, examples)}{suffix}";
        return new PromptTemplate(new PromptTemplateInput(template, inputVariables.ToList()));
    }

    public static PromptTemplate FromTemplate(string template, PromptTemplateInput? options = null)
    {
        var names = new HashSet<string>();
        ParseTemplate(template, options?.TemplateFormat ?? TemplateFormatOptions.FString)
            .ForEach(node =>
            {
                if (node.Type == "variable")
                {
                    names.Add((node as VariableNode).Name);
                }
            });

        return new PromptTemplate(new PromptTemplateInput(template, names.ToList())
        {
            TemplateFormat = options?.TemplateFormat ?? TemplateFormatOptions.FString,
        });
    }

    public override async Task<BasePromptTemplate> Partial(PartialValues values)
    {
        PromptTemplateInput promptDict = new PromptTemplateInput(Template, InputVariables
            .Where(iv => !values.Value.ContainsKey(iv))
            .ToList(), PartialVariables)
        {
            TemplateFormat = TemplateFormat,
            ValidateTemplate = ValidateTemplate,
        };

        if (PartialVariables != null)
        {
            foreach (var kvp in PartialVariables)
            {
                promptDict.PartialVariables[kvp.Key] = kvp.Value;
            }
        }

        foreach (var kvp in values.Value)
        {
            promptDict.PartialVariables[kvp.Key] = kvp.Value;
        }

        return new PromptTemplate(promptDict);
    }

    public override SerializedBasePromptTemplate Serialize()
    {
        return new SerializedPromptTemplate
        {
            InputVariables = InputVariables.ToList(),
            Template = Template,
        };
    }

    public static async Task<PromptTemplate> Deserialize(SerializedPromptTemplate data)
    {
        if (string.IsNullOrEmpty(data.Template))
        {
            throw new Exception("Prompt template must have a template");
        }

        return new PromptTemplate(new PromptTemplateInput(data.Template, data.InputVariables)
        {
            // TemplateFormat = data.template_format
        });
    }

    public delegate string Interpolator(string template, Dictionary<string, object> inputValues);
    public delegate List<ParsedFStringNode> Parser(string template);

    public static Dictionary<TemplateFormatOptions, Interpolator> DefaultFormatterMapping = new Dictionary<TemplateFormatOptions, Interpolator>
    {
        { TemplateFormatOptions.FString, InterpolateFString },
        { TemplateFormatOptions.Jinja2, (_, __) => "" }
    };

    public static Dictionary<TemplateFormatOptions, Parser> DefaultParserMapping = new Dictionary<TemplateFormatOptions, Parser>
    {
        { TemplateFormatOptions.FString, ParseFString },
        { TemplateFormatOptions.Jinja2, _ => new List<ParsedFStringNode>() }
    };
    
    public static string InterpolateFString(string template, Dictionary<string, object> values)
    {
        List<ParsedFStringNode> nodes = ParseFString(template);
        return nodes.Aggregate("", (res, node) =>
        {
            if (node.Type == "variable")
            {
                var parsedNode = node as VariableNode;
                
                if (values.ContainsKey(parsedNode.Name))
                {
                    return res + values[parsedNode.Name];
                }
                
                throw new ArgumentException($"Missing value for input {parsedNode.Name}");
            }

            return res + (node as LiteralNode).Text;
        });
    }
    
    public static List<ParsedFStringNode> ParseFString(string template)
    {
        // Core logic replicated from internals of pythons built in Formatter class.
        // https://github.com/python/cpython/blob/135ec7cefbaffd516b77362ad2b2ad1025af462e/Objects/stringlib/unicode_format.h#L700-L706
        List<char> chars = template.ToList();
        List<ParsedFStringNode> nodes = new List<ParsedFStringNode>();

        Func<string, int, int> nextBracket = (bracket, start) =>
        {
            for (int i = start; i < chars.Count; i++)
            {
                if (bracket.Contains(chars[i]))
                {
                    return i;
                }
            }
            return -1;
        };

        int i = 0;
        while (i < chars.Count)
        {
            if (chars[i] == '{' && i + 1 < chars.Count && chars[i + 1] == '{')
            {
                nodes.Add(new LiteralNode("{"));
                i += 2;
            }
            else if (chars[i] == '}' && i + 1 < chars.Count && chars[i + 1] == '}')
            {
                nodes.Add(new LiteralNode("}"));
                i += 2;
            }
            else if (chars[i] == '{')
            {
                int j = nextBracket("}", i);
                if (j < 0)
                {
                    throw new Exception("Unclosed '{' in template.");
                }

                nodes.Add(new VariableNode(new string(chars.GetRange(i + 1, j - (i + 1)).ToArray())));
                i = j + 1;
            }
            else if (chars[i] == '}')
            {
                throw new Exception("Single '}' in template.");
            }
            else
            {
                int next = nextBracket("{}", i);
                string text = next < 0 ? new string(chars.GetRange(i, chars.Count - i).ToArray()) : new string(chars.GetRange(i, next - i).ToArray());
                nodes.Add(new LiteralNode(text));
                i = next < 0 ? chars.Count : next;
            }
        }
        return nodes;
    }


    public static string RenderTemplate(string template, TemplateFormatOptions templateFormat, Dictionary<string, object> inputValues)
    {
        return DefaultFormatterMapping[templateFormat](template, inputValues);
    }

    
    public void CheckValidTemplate(string template, TemplateFormatOptions templateFormatOptions, List<string> inputVariables)
    {
        if (!DefaultFormatterMapping.ContainsKey(templateFormatOptions))
        {
            var validFormats = DefaultFormatterMapping.Keys;
            throw new Exception($"Invalid template format. Got `{templateFormatOptions}`; should be one of {string.Join(",", validFormats)}");
        }

        try
        {
            var dummyInputs = inputVariables.ToDictionary(v => v, v => new object());
            RenderTemplate(template, templateFormatOptions, dummyInputs);
        }
        catch
        {
            throw new Exception("Invalid prompt schema.");
        }
    }
    
    public static List<ParsedFStringNode> ParseTemplate(string template, TemplateFormatOptions templateFormat)
    {
        return DefaultParserMapping[templateFormat](template);
    }
}
