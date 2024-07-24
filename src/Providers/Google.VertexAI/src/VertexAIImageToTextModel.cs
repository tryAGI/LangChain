using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using System.Diagnostics;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAIImageToTextModel(VertexAIProvider provider, string id) : ImageToTextModel(id), IImageToTextModel
    {
        private VertexAIProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public override async Task<ImageToTextResponse> GenerateTextFromImageAsync(ImageToTextRequest request, ImageToTextSettings? settings = null, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var serviceAccountCredential = Provider.Configuration.GoogleCredential?.UnderlyingCredential as ServiceAccountCredential;
            var generateContentRequest = new GenerateContentRequest
            {
                Model = $"projects/{serviceAccountCredential?.ProjectId}/locations/{Provider.Configuration.Location}/publishers/{Provider.Configuration.Publisher}/models/{Id}",
                GenerationConfig = provider.Configuration.GenerationConfig,
                Contents =
                {
                    new Content
                    {
                        Role = "USER",
                        Parts =
                        {
                            new Part { Text = request.Prompt },
                            new Part { InlineData = new() { MimeType = "image/png", Data = ByteString.CopyFrom(request.Image.ToArray()) } }
                        }
                    }
                }
            };

            var watch = Stopwatch.StartNew();
            var response = await Provider.Api.GenerateContentAsync(generateContentRequest).ConfigureAwait(false);

            var usage = Usage.Empty with
            {
                OutputTokens = response.UsageMetadata.TotalTokenCount,
                InputTokens = response.UsageMetadata.PromptTokenCount,
                Time = watch.Elapsed
            };

            return new ImageToTextResponse
            {
                Text = response.Candidates[0].Content.Parts[0].Text,
                Usage = usage,
                UsedSettings = settings ?? ImageToTextSettings.Default
            };
        }

    }
}
