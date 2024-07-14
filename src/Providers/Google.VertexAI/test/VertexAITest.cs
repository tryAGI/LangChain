using Google.Apis.Auth.OAuth2;
using LangChain.Providers.Google.VertexAI.Predefined;

namespace LangChain.Providers.Google.VertexAI.Test
{
    [TestFixture]
    [Explicit]
    public partial class VertexAITests
    {
        [Test]
        public async Task Chat()
        {

            //Required 'GOOGLE_APPLICATION_CREDENTIALS' env with  Google credentials path json file.

            var credentials = GoogleCredential.GetApplicationDefault();

            if (credentials.UnderlyingCredential is ServiceAccountCredential serviceAccountCredential)
            {

                var config = new VertexAIConfiguration()
                {
                    ProjectId = serviceAccountCredential.ProjectId,
                    //Publisher = "google",
                    //Location = "us-central1",
                    /*GenerationConfig = new GenerationConfig
                    {
                        Temperature = 0.4f,
                        TopP = 1,
                        TopK = 32,
                        MaxOutputTokens = 2048
                    }*/
                };

                var provider = new VertexAIProvider(config);
                var model = new Gemini15ProModel(provider);

                string answer = await model.GenerateAsync("Generate some random name:");

                answer.Should().NotBeNull();

                Console.WriteLine(answer);
            }

        }
    }
}