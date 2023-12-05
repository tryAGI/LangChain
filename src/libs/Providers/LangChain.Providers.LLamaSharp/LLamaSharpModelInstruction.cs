using LLama.Common;
using LLama;
using System.Diagnostics;

namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
public class LLamaSharpModelInstruction : LLamaSharpModelBase
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
    
    /// <summary>
    /// Occurs when token generated.
    /// </summary>
    public event Action<string> TokenGenerated = delegate { };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var prompt = ToPrompt(request.Messages) + "\n";

        var watch = Stopwatch.StartNew();


        var context = Model.CreateContext(Parameters);
        var ex = new InstructExecutor(context);

        var inferenceParams = new InferenceParams()
        {
            Temperature = Configuration.Temperature,
            AntiPrompts = Configuration.AntiPrompts,
            MaxTokens = Configuration.MaxTokens,

        };


        var buf = "";
        await foreach (var text in ex.InferAsync(prompt,
                           inferenceParams, cancellationToken))
        {
            buf += text;
            TokenGenerated(text);
        }

        buf = LLamaSharpModelInstruction.SanitizeOutput(buf);
        var result = request.Messages.ToList();
        result.Add(buf.AsAiMessage());

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        TotalUsage += usage;

        return new ChatResponse(
            Messages: result,
            Usage: usage);
    }
}