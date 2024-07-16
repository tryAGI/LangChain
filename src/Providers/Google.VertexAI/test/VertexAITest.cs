using Google.Apis.Auth.OAuth2;
using LangChain.Providers.Google.VertexAI.Predefined;

namespace LangChain.Providers.Google.VertexAI.Test
{
    [TestFixture]
    [Explicit]
    //Required 'GOOGLE_APPLICATION_CREDENTIALS' env with  Google credentials path json file.
    public partial class VertexAITests
    {
        [Test]
        public async Task Chat()
        {

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
                var model = new Gemini15ProChatModel(provider);

                string answer = await model.GenerateAsync("Generate some random name:");

                answer.Should().NotBeNull();

                Console.WriteLine(answer);
            }

        }

        [Test]
        public async Task TextToImage()
        {

            var credentials = GoogleCredential.GetApplicationDefault();

            if (credentials.UnderlyingCredential is ServiceAccountCredential serviceAccountCredential)
            {

                var config = new VertexAIConfiguration()
                {
                    ProjectId = serviceAccountCredential.ProjectId
                };

                var provider = new VertexAIProvider(config);

                var model = new VertexAITextToImageModel(provider, "imagegeneration@006", 2);

                var answer = await model.GenerateImageAsync("a dog reading a newspaper");

                answer.Should().NotBeNull();

                foreach(var img in answer.Images)
                {
                    string outputFileName = $"dog_newspaper_{Guid.NewGuid()}.png";
                    File.WriteAllBytes(outputFileName, Convert.FromBase64String(img));
                    FileInfo fileInfo = new(Path.GetFullPath(outputFileName));
                    Console.WriteLine($"Created output image {fileInfo.FullName} with {fileInfo.Length} bytes");
                }
            }
        }
    }
}