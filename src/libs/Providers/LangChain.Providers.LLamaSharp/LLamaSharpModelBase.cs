using LLama.Common;
using LLama;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
public abstract class LLamaSharpModelBase : IChatModel
{
    /// <summary>
    /// 
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public Usage TotalUsage { get; protected set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int ContextLength => Configuration.ContextSize;

    /// <summary>
    /// 
    /// </summary>
    public LLamaSharpConfiguration Configuration { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected LLamaWeights Model { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public ModelParams Parameters { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected LLamaSharpModelBase(LLamaSharpConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,

        };
        Model = LLamaWeights.LoadFromFile(Parameters);
        Configuration = configuration;
        Id = Path.GetFileNameWithoutExtension(configuration.PathToModelFile);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected static string ConvertRole(MessageRole role)
    {
        return role switch
        {
            MessageRole.Human => "Human: ",
            MessageRole.Ai => "Assistant: ",
            MessageRole.System => "",
            _ => throw new NotSupportedException($"the role {role} is not supported")
        };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected static string ConvertMessage(Message message)
    {
        return $"{ConvertRole(message.Role)}{message.Content}";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    protected static string ToPrompt(IEnumerable<Message> messages)
    {
        return string.Join("\n", messages.Select(ConvertMessage).ToArray());
    }
}