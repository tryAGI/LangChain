using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;
using LangChain.Providers;
using LangChain.Providers.OpenAI.Predefined;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task AgentWithOllamaReact()
    {
        // var provider = new OllamaProvider(
        //     options: new RequestOptions
        //     {
        //         Stop = new[] { "Observation", "[END]" }, // add injection word `Observation` and `[END]` to stop the model(just as additional safety feature)
        //         Temperature = 0
        //     });
        //var llm = new OllamaChatModel(provider, id: "mistral:latest").UseConsoleForDebug();
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InvalidOperationException("OpenAI API key is not set");
        var llm = new Gpt35TurboModel(apiKey).UseConsoleForDebug();

        // create a google search 
        var searchApiKey =
            Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY") ??
            throw new InvalidOperationException("GOOGLE_SEARCH_API_KEY is not set");
        var cx =
            Environment.GetEnvironmentVariable("GOOGLE_SEARCH_CX") ??
            throw new InvalidOperationException("GOOGLE_SEARCH_CX is not set");
        var searchTool = new GoogleCustomSearchTool(key: searchApiKey, cx: cx, resultsLimit: 1);

        var chain =
            Set("What is tryAGI/LangChain?")
            | ReActAgentExecutor(llm) // does the magic
                .UseTool(searchTool); // add the google search tool

        await chain.RunAsync();
    }
}