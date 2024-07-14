using Google.Cloud.AIPlatform.V1;

namespace LangChain.Providers.Google.VertexAI
{

    public class VertexAIProvider : Provider
    {
        public PredictionServiceClient Api { get; private set; }
        public VertexAIConfiguration Configuration { get; private set; }
        public VertexAIProvider(VertexAIConfiguration configuration) : base(id: VertexAIConfiguration.SectionName)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Api = new PredictionServiceClientBuilder
            {
                Endpoint = $"{Configuration.Location}-aiplatform.googleapis.com"
            }.Build();
        }
    }
}
