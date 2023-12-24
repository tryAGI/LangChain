using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class ChatPromptTemplate : BaseChatPromptTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public List<BaseMessagePromptTemplate> PromptMessages { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ValidateTemplate { get; private set; }

    /// <inheritdoc/>
    public ChatPromptTemplate(ChatPromptTemplateInput input) : base(input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        
        this.PromptMessages = input.PromptMessages;
        this.ValidateTemplate = input.ValidateTemplate;

        if (this.ValidateTemplate)
        {
            this.validateInputs(input);
        }
    }

    /// <inheritdoc/>
    public override Task<BasePromptTemplate> AddPartial(PartialValues values)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override string GetPromptType()
    {
        return "chat";
    }

    /// <inheritdoc/>
    public override SerializedBasePromptTemplate Serialize()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyCollection<Message>> FormatMessages(InputValues values)
    {
        var allValues = await this.MergePartialAndUserVariables(values).ConfigureAwait(false);

        var resultMessages = new List<Message>();

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

            var message = await promptMessage.FormatMessages(inputValues).ConfigureAwait(false);
            resultMessages.AddRange(message);
        }

        return resultMessages.ToArray();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="promptMessages"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ChatPromptTemplate FromPromptMessages(List<BaseMessagePromptTemplate> promptMessages)
    {
        promptMessages = promptMessages ?? throw new ArgumentNullException(nameof(promptMessages));
        
        var flattenedMessages = new List<BaseMessagePromptTemplate>();

        foreach (var promptMessage in promptMessages)
        {
            flattenedMessages.Add(promptMessage);
        }

        var inputVariables = new HashSet<string>();

        return new ChatPromptTemplate(new ChatPromptTemplateInput()
        {
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

