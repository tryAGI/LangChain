using System.Diagnostics;
using LLama;
using System.Reflection;
using LLama.Common;

namespace LangChain.Providers.LLamaSharp
{
    public class LLamaSharpModelChat : LLamaSharpModelBase
    {
        public LLamaSharpModelChat(LLamaSharpConfiguration configuration) : base(configuration)
        {
        }


        string SanitizeOutput(string res)
        {
            return res.Replace("Human:", "").Replace("Assistant:", "").Trim();
        }

        public override async Task<ChatResponse> GenerateAsync(ChatRequest request,
            CancellationToken cancellationToken = default)
        {
            var prompt = ToPrompt(request.Messages);

            var watch = Stopwatch.StartNew();


            var context = _model.CreateContext(_parameters);
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);
            var inferenceParams = new InferenceParams()
            {
                Temperature = _configuration.Temperature,
                AntiPrompts = new List<string> { "Human:" },
                MaxTokens = _configuration.MaxTokens,

            };


            var buf = "";
            await foreach (var text in session.ChatAsync(prompt,
                               inferenceParams, cancellationToken))
            {
                buf += text;
            }

            buf = SanitizeOutput(buf);
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
}