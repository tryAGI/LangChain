using LLama.Common;
using LLama;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
public abstract class LLamaSharpModelBase
    : ChatModel
{
    /// <summary>
    /// 
    /// </summary>
    public Usage TotalUsage { get; protected set; }
    
    /// <summary>
    /// 
    /// </summary>
    public override int ContextLength => Configuration.ContextSize;

    /// <summary>
    /// 
    /// </summary>
    public LLamaSharpConfiguration Configuration { get; }
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    protected LLamaWeights Model { get; }
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public ModelParams Parameters { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected LLamaSharpModelBase(LLamaSharpConfiguration configuration)
        : base(id: Path.GetFileNameWithoutExtension(configuration?.PathToModelFile ?? string.Empty))
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Parameters = new ModelParams(configuration.PathToModelFile)
        {
            ContextSize = (uint)configuration.ContextSize,
            Seed = (uint)configuration.Seed,

        };
        Model = LLamaWeights.LoadFromFile(Parameters);
        Configuration = configuration;
    }

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