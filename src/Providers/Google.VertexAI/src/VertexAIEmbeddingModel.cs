using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;
using System.Diagnostics;
using Value = Google.Protobuf.WellKnownTypes.Value;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAIEmbeddingModel(
    VertexAIProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
    {
        private VertexAIProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public async Task<EmbeddingResponse> CreateEmbeddingsAsync(EmbeddingRequest request, EmbeddingSettings? settings = null, CancellationToken cancellationToken = default)
        {

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var serviceAccountCredential = Provider.Configuration.GoogleCredential?.UnderlyingCredential as ServiceAccountCredential;
            var endpoint = EndpointName.FromProjectLocationPublisherModel(
                    serviceAccountCredential?.ProjectId,
                    Provider.Configuration.Location,
                    Provider.Configuration.Publisher, Id);

            var watch = Stopwatch.StartNew();
            var embeddings = new List<float[]>(capacity: request.Strings.Count);
            var tasks = request.Strings.Select(text => provider.Api.PredictAsync(endpoint,
                [
                    Value.ForStruct(new()
                    {
                        Fields =
                        {
                            ["content"] = Value.ForString(text),
                        }
                    })
                ], null))
            .ToList();

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            var values = results.Select(static x => x.Predictions.First()
            .StructValue.Fields["embeddings"]
            .StructValue.Fields["values"]
            .ListValue.Values
            .Select(static x => (float)x.NumberValue).ToArray()).ToArray();

            var usage = Usage.Empty with
            {
                Time = watch.Elapsed
            };

            return new EmbeddingResponse
            {
                Values = values,
                UsedSettings = settings ?? EmbeddingSettings.Default,
                Usage = usage,
                Dimensions = values?.Length ?? 0,
            };
        }
    }
}
