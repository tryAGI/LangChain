using System.Diagnostics;

namespace LangChain.Providers.Bedrock.Models
{
    public class BedrockModel : BedrockModelBase
    {
        private readonly BedrockConfiguration _configuration;

        public BedrockModel(string modelId)
        {
            Id = modelId ?? throw new ArgumentException("ModelId is not defined", nameof(modelId));
            _configuration = new BedrockConfiguration { ModelId = modelId };
        }

        public override async Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var watch = Stopwatch.StartNew();

            var response = await CreateChatCompletionAsync(request, _configuration, cancellationToken).ConfigureAwait(false);

            var result = request.Messages.ToList();
            result.Add(response.AsAiMessage());

            watch.Stop();

            // Unsupported
            var usage = Usage.Empty with
            {
                Time = watch.Elapsed,
            };
            lock (_usageLock)
            {
                TotalUsage += usage;
            }

            return new ChatResponse(Messages: result, Usage.Empty);
        }
    }
}
