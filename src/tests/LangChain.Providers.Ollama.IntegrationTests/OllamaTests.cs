using OllamaTest;

namespace LangChain.Providers.Ollama.IntegrationTests
{
    [TestFixture]
    public class OllamaTests
    {
        [Test]
        [Explicit]
        public void InstructionTest ()
        {
            OllamaLanguageModelInstruction model = new OllamaLanguageModelInstruction("mistral",options:new OllamaLanguageModelOptions(){Temperature = 0f,Stop = new []{"\n"} });
            var response = model.GenerateAsync(new ChatRequest(new List<Message>
            {
                @"You are a calculator. You print only numbers.
User: Print the result of this expression: 2 + 2.
Calculator:".AsSystemMessage(),
                
            })).Result;

            Assert.AreEqual("4", response.Messages.Last().Content.Trim());
        }
    }
}