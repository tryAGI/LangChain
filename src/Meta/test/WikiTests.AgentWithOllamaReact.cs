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
        //// # ReAct
        //// But how we can fix that? If you know about [RAG](https://tryagi.github.io/LangChain/wiki/RagWithOpenAiOllama/) then you know that there is some tricks to bring new abilities to LLM. And one of those tricks is [ReAct](https://www.promptingguide.ai/techniques/react) prompting.
        //// 
        //// In simple words ReAct is forcing LLM to reflect on your question and injects responses as if LLM figured them out by itself. This allows you to connect any datasource or `tool` tou your LLM.
        //// Let's try to use ReAct and connect Google search to your LLM.
        //// 
        //// ## Google custom search
        //// 
        //// LangChain has Google search built in. To use it you need to get `key` and `cx` from Google. Don't worry, it's free.
        //// 
        //// To get api `key` go here: https://developers.google.com/custom-search/v1/overview.
        //// You need to create Programmable Search Engine to get `cx`.
        //// 
        //// ## Using ReAct with Google search
        //// 
        //// Now you should have all necessary to connect your LLM to Google search

        // var provider = new OllamaProvider(
        //     options: new RequestOptions
        //     {
        //         Stop = new[] { "Observation", "[END]" }, // add injection word `Observation` and `[END]` to stop the model(just as additional safety feature)
        //         Temperature = 0
        //     });
        // var llm = new OllamaChatModel(provider, id: "llama3.1").UseConsoleForDebug();
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InvalidOperationException("OPENAI_API_KEY key is not set");
        var llm = new OpenAiLatestFastChatModel(apiKey).UseConsoleForDebug();

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

        //// Lets run it and see the output:
        //// As you can see, instead of giving answer right away, the model starts to think on it
        //// ```
        //// Question: What is tryAGI/LangChain?
        //// Thought: I don't know much about tryAGI or LangChain, so I need to search for more information.
        //// Action: google
        //// Action Input: tryAGI LangChain
        //// 
        //// Observation:
        //// ```
        //// Here is where magic occurs. The model stops vecause of the stop word and ReAct agent kicks in. It sees that model wants to use google and look for "tryAGI LangChain". So it does exactly this and puts search results back to the model:
        //// ```
        //// Observation: tryAGI/LangChain: C# implementation of LangChain. We try ... - GitHub
        //// C# implementation of LangChain. We try to be as close to the original as possible in terms of abstractions, but are open to new entities.
        //// Source url: https://github.com/tryAGI/LangChain
        //// ```
        //// So now model sees the result of it's action so it continues it's reflection:
        //// ```
        //// Thought: Based on the observation, I now have a better understanding of what tryAGI/LangChain is.
        //// Final Answer: tryAGI/LangChain is an open-source C# implementation of LangChain. It aims to be as close to the original abstractions as possible but is also open to new entities.
        //// ```
        //// The `Final answer` is actually correct.
    }
}