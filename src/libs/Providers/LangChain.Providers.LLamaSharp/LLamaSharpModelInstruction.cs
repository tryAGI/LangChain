using LLama.Common;
using LLama;
using System.Diagnostics;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
public class LLamaSharpModelInstruction : LLamaSharpModelBase, IChatModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="temperature"></param>
    /// <returns></returns>
    public static LLamaSharpModelInstruction FromPath(string path, float temperature = 0)
    {
        return new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = path,
            Temperature = temperature
        });

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public LLamaSharpModelInstruction(LLamaSharpConfiguration configuration) : base(configuration)
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="res"></param>
    /// <returns></returns>
    private static string SanitizeOutput(string res)
    {
        return res
            .Replace("\n>", string.Empty)
            .Trim();
    }
    
    /// <inheritdoc />
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var prompt = ToPrompt(request.Messages);

        var watch = Stopwatch.StartNew();


        var context = Model.CreateContext(Parameters);
        var ex = new InstructExecutor(context);

        var inferenceParams = new InferenceParams()
        {
            Temperature = Configuration.Temperature,
            AntiPrompts = Configuration.AntiPrompts,
            MaxTokens = Configuration.MaxTokens,
            RepeatPenalty = Configuration.RepeatPenalty
        };

        OnPromptSent(prompt);
        
        var buf = "";
        await foreach (var text in ex.InferAsync(prompt,
                           inferenceParams, cancellationToken))
        {
            buf += text;
            foreach (string antiPrompt in Configuration.AntiPrompts)
            {
                if (buf.EndsWith(antiPrompt, StringComparison.Ordinal))
                {
                    buf = buf.Substring(0, buf.Length - antiPrompt.Length);
                    break;
                }
            }
            
            OnPartialResponseGenerated(text);
        }

        buf = SanitizeOutput(buf);
        OnCompletedResponseGenerated(buf);
        
        var result = request.Messages.ToList();
        result.Add(buf.AsAiMessage());

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        TotalUsage += usage;

        return new ChatResponse
        {
            Messages = result,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
    }
}