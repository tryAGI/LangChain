using LLama.Common;
using LLama;

namespace LangChain.Providers.LLamaSharp;

public abstract class LLamaSharpModelBase:IChatModel
{
    public string Id { get; }
    public Usage TotalUsage { get; protected set; }
    public int ContextLength =>_configuration.ContextSize;

    protected readonly LLamaSharpConfiguration _configuration;
    protected readonly LLamaWeights _model;
    protected readonly ModelParams _parameters;

    protected LLamaSharpModelBase(LLamaSharpConfiguration configuration)
    {
        _parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,

        };
        _model = LLamaWeights.LoadFromFile(_parameters);
        _configuration = configuration;
        Id = Path.GetFileNameWithoutExtension(configuration.PathToModelFile);
    }

    public abstract Task<ChatResponse>
        GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default);

    protected string ConvertRole(MessageRole role)
    {
        return role switch
        {
            MessageRole.Human => "Human: ",
            MessageRole.Ai => "Assistant: ",
            MessageRole.System => "",
            _ => throw new NotSupportedException($"the role {role} is not supported")
        };
    }
    protected string ConvertMessage(Message message)
    {
        return $"{ConvertRole(message.Role)}{message.Content}";
    }
    protected string ToPrompt(IEnumerable<Message> messages)
    {
        return string.Join("\n", messages.Select(ConvertMessage).ToArray());
    }
}