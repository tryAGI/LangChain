using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Providers;
using LangChain.Prompts;
using LangChain.Schema;

using var httpClient = new HttpClient();
var llm = new Gpt35TurboModel("api-key", httpClient);

var firstTemplate = "What is a good name for a company that makes {product}?";
var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1){"product"}));

var chainOne = new LlmChain(new LlmChainInput(llm, firstPrompt)
{
    OutputKey = "company_name"
});

var secondTemplate = "Write a 20 words description for the following company:{company_name}";
var secondPrompt = new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1){"company_name"}));

var chainTwo = new LlmChain(new LlmChainInput(llm, secondPrompt));

var overallChain = new SequentialChain(new SequentialChainInput(new []
{
    chainOne,
    chainTwo
}, new []{"product"}));

var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
{
    { "product", "colourful socks" }
}));

Console.WriteLine(result.Value["text"]);
Console.WriteLine("Test");