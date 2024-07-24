using Google.Apis.Auth.OAuth2;
using LangChain.Providers.Google.VertexAI.Predefined;

namespace LangChain.Providers.Google.VertexAI.Test
{
    [TestFixture]
    [Explicit]
    //Required 'GOOGLE_APPLICATION_CREDENTIALS' env with Google credentials path json file.
    public partial class VertexAITests
    {
        [Test]
        public async Task Chat()
        {
            var config = new VertexAIConfiguration()
            {
                //Publisher = "google",
                //Location = "us-central1",
                //GoogleCredential = GoogleCredential.FromJson("{your-json}"),
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

        [Test]
        public async Task TextToImage()
        {
            var provider = new VertexAIProvider(new VertexAIConfiguration());
            var model = new VertexAITextToImageModel(provider, "imagegeneration@006", 2);
            var answer = await model.GenerateImageAsync("a dog reading a newspaper");
            answer.Should().NotBeNull();

            foreach (var img in answer.Images)
            {
                string outputFileName = $"dog_newspaper_{Guid.NewGuid()}.png";
                File.WriteAllBytes(outputFileName, Convert.FromBase64String(img));
                FileInfo fileInfo = new(Path.GetFullPath(outputFileName));
                Console.WriteLine($"Created output image {fileInfo.FullName} with {fileInfo.Length} bytes");
            }
        }

        [Test]
        public async Task ImageToText()
        {
            var imageb64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAApgAAAKYB3X3/OAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAANCSURBVEiJtZZPbBtFFMZ/M7ubXdtdb1xSFyeilBapySVU8h8OoFaooFSqiihIVIpQBKci6KEg9Q6H9kovIHoCIVQJJCKE1ENFjnAgcaSGC6rEnxBwA04Tx43t2FnvDAfjkNibxgHxnWb2e/u992bee7tCa00YFsffekFY+nUzFtjW0LrvjRXrCDIAaPLlW0nHL0SsZtVoaF98mLrx3pdhOqLtYPHChahZcYYO7KvPFxvRl5XPp1sN3adWiD1ZAqD6XYK1b/dvE5IWryTt2udLFedwc1+9kLp+vbbpoDh+6TklxBeAi9TL0taeWpdmZzQDry0AcO+jQ12RyohqqoYoo8RDwJrU+qXkjWtfi8Xxt58BdQuwQs9qC/afLwCw8tnQbqYAPsgxE1S6F3EAIXux2oQFKm0ihMsOF71dHYx+f3NND68ghCu1YIoePPQN1pGRABkJ6Bus96CutRZMydTl+TvuiRW1m3n0eDl0vRPcEysqdXn+jsQPsrHMquGeXEaY4Yk4wxWcY5V/9scqOMOVUFthatyTy8QyqwZ+kDURKoMWxNKr2EeqVKcTNOajqKoBgOE28U4tdQl5p5bwCw7BWquaZSzAPlwjlithJtp3pTImSqQRrb2Z8PHGigD4RZuNX6JYj6wj7O4TFLbCO/Mn/m8R+h6rYSUb3ekokRY6f/YukArN979jcW+V/S8g0eT/N3VN3kTqWbQ428m9/8k0P/1aIhF36PccEl6EhOcAUCrXKZXXWS3XKd2vc/TRBG9O5ELC17MmWubD2nKhUKZa26Ba2+D3P+4/MNCFwg59oWVeYhkzgN/JDR8deKBoD7Y+ljEjGZ0sosXVTvbc6RHirr2reNy1OXd6pJsQ+gqjk8VWFYmHrwBzW/n+uMPFiRwHB2I7ih8ciHFxIkd/3Omk5tCDV1t+2nNu5sxxpDFNx+huNhVT3/zMDz8usXC3ddaHBj1GHj/As08fwTS7Kt1HBTmyN29vdwAw+/wbwLVOJ3uAD1wi/dUH7Qei66PfyuRj4Ik9is+hglfbkbfR3cnZm7chlUWLdwmprtCohX4HUtlOcQjLYCu+fzGJH2QRKvP3UNz8bWk1qMxjGTOMThZ3kvgLI5AzFfo379UAAAAASUVORK5CYII=";
            var provider = new VertexAIProvider(new VertexAIConfiguration { GoogleCredential = GoogleCredential.GetApplicationDefault() });
            var model = new Gemini15FlashImageToTextModel(provider);
            var request = new ImageToTextRequest {
                Image = BinaryData.FromBytes(Convert.FromBase64String(imageb64)),
                Prompt = "What's in this photo?"
            };
            var answer = await model.GenerateTextFromImageAsync(request);
            answer.Should().NotBeNull();
            Console.WriteLine(answer.Text);
        }
    }
}