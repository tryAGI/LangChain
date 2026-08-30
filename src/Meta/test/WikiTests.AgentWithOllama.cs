using Meai = Microsoft.Extensions.AI;
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
        //// You would need `LangChain` NuGet package. It's a meta package that includes all necessary dependencies(`LangChain.Core` and `Ollama`):
        //// And ollama with `llama3.1` running on your computer.
        ////
        //// We will start with basic ollama setup and simple question to the LLM:
        using var client = new OllamaApiClient(
            // baseUri: new Uri("http://172.16.50.107:11434") // if you have ollama running on different computer/port. Default is "http://localhost:11434"
            );
        // TODO: OllamaApiClient (Ollama NuGet 1.15.0) does not implement Microsoft.Extensions.AI.IChatClient.
        // Update when Ollama NuGet adds MEAI support. This cast will throw InvalidCastException at runtime.
        Meai.IChatClient model = (Meai.IChatClient)(object)client;

        var chain =
            Set("What is tryAGI/LangChain? In 5 words")
            | LLM(model, options: new Meai.ChatOptions { ModelId = "llama3.1" });

        await chain.RunAsync();
        //// In the console you will see pretty general answer:
        //// ```
        //// tryAGI/LangChain is a research project focused on developing an advanced conversational AI system that can understand and generate human-like text in multiple languages. The goal is to create a versatile, multilingual language model that can engage in complex conversations, learn from its interactions, and adapt to new contexts.
        //// ```
        //// So, basically, `llama3.1` guesses the answer. It has no idea what you are asking about.  
    }
}