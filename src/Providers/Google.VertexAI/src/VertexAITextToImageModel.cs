using Google.Cloud.AIPlatform.V1;
using System.Diagnostics;
using Value = Google.Protobuf.WellKnownTypes.Value;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAITextToImageModel(VertexAIProvider provider,
        string id, int sampleCount = 1) : TextToImageModel(id), ITextToImageModel
    {
        private VertexAIProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public async Task<TextToImageResponse> GenerateImageAsync(TextToImageRequest request, TextToImageSettings? settings = null, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var watch = Stopwatch.StartNew();
            var predictRequest = new PredictRequest
            {
                EndpointAsEndpointName = EndpointName.FromProjectLocationPublisherModel(
                    Provider.Configuration.ProjectId,
                    Provider.Configuration.Location,
                    Provider.Configuration.Publisher, Id),
                Instances =
                {
                    Value.ForStruct(new()
                    {
                        Fields =
                        {
                            ["prompt"] = Value.ForString(request.Prompt)
                        }
                    })
                },
                Parameters = Value.ForStruct(new()
                {
                    Fields =
                    {
                        ["sampleCount"] = Value.ForNumber(sampleCount)
                    }
                })
            };

            var response = await provider.Api.PredictAsync(predictRequest).ConfigureAwait(true);

            var images = response.Predictions
                .Select(prediction => Data.FromBase64(prediction.StructValue.Fields["bytesBase64Encoded"].StringValue))
                .ToList();

            var usage = Usage.Empty with
            {
                Time = watch.Elapsed,
            };

            return new TextToImageResponse
            {
                Images = images,
                UsedSettings = settings ?? TextToImageSettings.Default,
                Usage = usage,
            };
        }
    }
}
