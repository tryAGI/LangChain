namespace LangChain.Providers.Ollama.IntegrationTests;

[TestFixture]
public class OllamaTests
{
    [Test]
    [Explicit]
    public async Task InstructionTest()
    {
        var model = new OllamaChatModel(new OllamaProvider(), id: "mistral")
        {
            Settings = new ChatSettings
            {
                //Temperature = 0.0,
                StopSequences = ["\n"],
            }
        };
        var response = await model.GenerateAsync("""
                                                 You are a calculator. You print only numbers.
                                                 User: Print the result of this expression: 2 + 2.
                                                 Calculator:
                                                 """);

        Assert.AreEqual("4", response.Messages.Last().Content.Trim());
    }
}