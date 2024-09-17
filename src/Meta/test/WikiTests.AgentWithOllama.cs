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
        //// # Introduction
        //// By nature LLM's are restricted to text generation. So there is no access to internet, your computer files or your inner network to do something usefull.
        //// Usually information that you can ask about is limited by LLM's learning date. Everything that occured after this date is a mistery for it.
        //// 
        //// It's easy to check.
        //// You would need `LangChain` NuGet package. It's a meta package that includes all necessary dependencies(`LangChain.Core` and `LangChain.Providers.Ollama`):
        //// And ollama with `llama3.1` running on your computer.
        //// 
        //// We will start with basic ollama setup and simple question to the LLM:
        var provider = new OllamaProvider(
            // url: "http://172.16.50.107:11434", // if you have ollama running on different computer/port. Default is "http://localhost:11434/api"
            );
        var model = new OllamaChatModel(provider, id: "llama3.1").UseConsoleForDebug();

        var chain =
            Set("What is tryAGI/LangChain? In 5 words")
            | LLM(model);

        await chain.RunAsync();
        //// In the console you will see pretty general answer:
        //// ```
        //// tryAGI/LangChain is a research project focused on developing an advanced conversational AI system that can understand and generate human-like text in multiple languages. The goal is to create a versatile, multilingual language model that can engage in complex conversations, learn from its interactions, and adapt to new contexts.
        //// ```
        //// So, basically, `llama3.1` guesses the answer. It has no idea what you are asking about.  
    }
}