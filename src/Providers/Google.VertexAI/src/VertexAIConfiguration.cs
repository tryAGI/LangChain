using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAIConfiguration
    {
        public const string SectionName = "VertexAI";
        public string Location { get; set; } = "us-central1";
        public string Publisher { get; set; } = "google";
        public GoogleCredential? GoogleCredential { get; set; } = GoogleCredential.GetApplicationDefault();
        public GenerationConfig? GenerationConfig { get; set; }
    }
}