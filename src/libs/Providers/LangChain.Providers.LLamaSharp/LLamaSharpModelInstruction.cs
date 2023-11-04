using LLama.Common;
using LLama;
using System.Diagnostics;

namespace LangChain.Providers.LLamaSharp;

public class LLamaSharpModelInstruction:LLamaSharpModelBase
{
    public LLamaSharpModelInstruction(LLamaSharpConfiguration configuration) : base(configuration)
    {
    }
    string SanitizeOutput(string res)
    {
        return res.Replace("\n>", "").Trim();
    }
    public override async Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var prompt = ToPrompt(request.Messages)+"\n";

        var watch = Stopwatch.StartNew();


        var context = _model.CreateContext(_parameters);
        var ex = new InstructExecutor(context);
     
        var inferenceParams = new InferenceParams()
        {
            Temperature = _configuration.Temperature,
            AntiPrompts = new List<string> { ">" },
            MaxTokens = _configuration.MaxTokens,

        };


        var buf = "";
        await foreach (var text in ex.InferAsync(prompt,
                           inferenceParams, cancellationToken))
        {
            buf += text;
        }

        buf=SanitizeOutput(buf);
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