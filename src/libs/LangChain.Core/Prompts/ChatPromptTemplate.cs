using LangChain.NET.Chat;
using LangChain.NET.Prompts.Base;
using LangChain.NET.Schema;

namespace LangChain.NET.Prompts;

public class ChatPromptTemplate : BaseChatPromptTemplate
{
    public List<BaseMessagePromptTemplate> PromptMessages { get; private set; }

    public bool ValidateTemplate { get; private set; }

    public ChatPromptTemplate(ChatPromptTemplateInput input) : base(input)
    {
        this.PromptMessages = input.PromptMessages;
        this.ValidateTemplate = input.ValidateTemplate;

        if (this.ValidateTemplate)
        {
            this.validateInputs(input);
        }
    }

    public override Task<BasePromptTemplate> Partial(PartialValues values)
    {
        throw new NotImplementedException();
    }

    protected override string GetPromptType()
    {
        return "chat";
    }

    public override SerializedBasePromptTemplate Serialize()
    {
        throw new NotImplementedException();
    }

    public override async Task<BaseChatMessage[]> FormatMessages(InputValues values)
    {
        var allValues = await this.MergePartialAndUserVariables(values);

        var resultMessages = new List<BaseChatMessage>();

        foreach (var promptMessage in this.PromptMessages)
        {
            var inputValues = new InputValues(new Dictionary<string, object>());

            foreach (var inputVariable in promptMessage.InputVariables)
            {
                if (!allValues.Value.ContainsKey(inputVariable))
                {
                    throw new ArgumentException($"Missing value for input variable `{inputVariable}`");
                }

                inputValues.Value.Add(inputVariable, allValues.Value[inputVariable]);
            }

            var message = await promptMessage.FormatMessages(inputValues);
            resultMessages.AddRange(message);
        }

        return resultMessages.ToArray();
    }
    
    
    public static ChatPromptTemplate FromPromptMessages(List<BaseMessagePromptTemplate> promptMessages)
    {
        var flattenedMessages = new List<BaseMessagePromptTemplate>();

        foreach (var promptMessage in promptMessages)
        {
            flattenedMessages.Add(promptMessage);
        }

        var inputVariables = new HashSet<string>();

        return new ChatPromptTemplate(new ChatPromptTemplateInput(){
            InputVariables = inputVariables.ToList(),
            PromptMessages = flattenedMessages,
            ValidateTemplate = false
        });
    }

    private void validateInputs(ChatPromptTemplateInput input)
    {
        var promptVariables = new List<string>();

        foreach (var prompt in input.PromptMessages)
        {
            foreach (var inputVariable in prompt.InputVariables)
            {
                if (!promptVariables.Contains(inputVariable))
                {
                    promptVariables.Add(inputVariable);
                }
            }
        }

        if (promptVariables.Count != input.InputVariables.Count)
        {
            var missingVariables = promptVariables.Except(input.InputVariables);

            throw new ArgumentException($"Input variables `{string.Join(",", missingVariables)}` are not used in any of the prompt messages.");
        }
    }
}

