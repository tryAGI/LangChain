using LangChain.Providers;
using LangChain.Providers.Ollama;
using Ollama;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public partial class WikiTests
{
    [Test]
    public async Task AgentWithOllama()
    {
        var provider = new OllamaProvider(
            // "http://172.16.50.107:11434",
            options: new RequestOptions
            {
                Temperature = 0,
            });
        var model = new OllamaChatModel(provider, id: "llama3").UseConsoleForDebug();

        var chain =
            Set("What is tryAGI/LangChain? In 5 words")
            | LLM(model);

        await chain.RunAsync();
    }
}