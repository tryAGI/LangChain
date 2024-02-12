using System;
using System.Diagnostics;

namespace LangChain.Providers.Amazon.SageMaker.Models
{
    public class SageMakerModel : SageMakerModelBase
    {
        private readonly SageMakerConfiguration _configuration;

        public SageMakerModel(string apiGatewayEndpoint, string sageMakerEndpointName)
        {
            Url = apiGatewayEndpoint ?? throw new ArgumentException("API Gateway Endpoint is not defined", nameof(apiGatewayEndpoint));
            Id = sageMakerEndpointName ?? throw new ArgumentException("SageMaker Endpoint Name is not defined", nameof(sageMakerEndpointName));
            _configuration = new SageMakerConfiguration { ModelId = Id, Url = Url };
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
