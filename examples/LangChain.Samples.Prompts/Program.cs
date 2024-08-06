using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Schema;

const string apiKey = "API-KEY";
using var httpClient = new HttpClient();
var llm = new OpenAiLatestFastChatModel(apiKey);

var template = "What is a good name for a company that makes {product}?";
var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

var chain = new LlmChain(new LlmChainInput(llm, prompt));

var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
{
    { "product", "colourful socks" }
}));

// The result is an object with a `text` property.
Console.WriteLine(result.Value["text"]);

// Since the LLMChain is a single-input, single-output chain, we can also call it with `run`.
// This takes in a string and returns the `text` property.
var result2 = await chain.RunAsync("colourful socks");

Console.WriteLine(result2);

// We can also construct an LLMChain from a ChatPromptTemplate and a chat model.
var chat = new OpenAiLatestFastChatModel(apiKey);

var chatPrompt = ChatPromptTemplate.FromPromptMessages(new List<BaseMessagePromptTemplate>(2)
{
    SystemMessagePromptTemplate.FromTemplate("You are a helpful assistant that translates {input_language} to {output_language}."),
    HumanMessagePromptTemplate.FromTemplate("{text}")
});

var chainB = new LlmChain(new LlmChainInput(chat, chatPrompt)
{
    Verbose = true
});

var resultB = await chainB.CallAsync(new ChainValues(new Dictionary<string, object>(3)
{
    {"input_language", "English"},
    {"output_language", "French"},
    {"text", "I love programming"},
}));

Console.WriteLine(resultB.Value["text"]);