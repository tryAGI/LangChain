using StableDiffusion;
using static LangChain.Chains.Chain;
namespace LangChain.Providers.Automatic1111.IntegrationTests
{
    [TestFixture]
    public class Automatic1111Tests
    {
        [Test]
        [Explicit]
        public void InstructionTest ()
        {
            Automatic1111Model model = new Automatic1111Model();
            
            var tempPath= Path.Combine(Path.GetTempPath(), "test.png");

            var chain =
                Set("Car", outputKey: "prompt")
                | GenerateImage(model, inputKey: "prompt", outputKey: "image")
                | SaveIntoFile(path: tempPath, inputKey: "image");
            
            chain.Run().Wait();
            
            Assert.Pass();
        }
    }
}