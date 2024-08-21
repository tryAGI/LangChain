using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task HowToUseOpenAiProvider()
    {
        //// To use models like GPT-3.5 or GPT-4 you would need OpenAI api key and model name.

        var model = new OpenAiLatestFastChatModel("your_openAI_key");
        var chain =
            Set("Hello!", outputKey: "request")          // set context variable `request` to "Hello"
            | LLM(model, inputKey: "request", outputKey: "text"); // get text from context variable `request`, pass it to the model and put result into `text`

        var result = await chain.RunAsync("text", CancellationToken.None);  // execute chain and get `text` context variable
        Console.WriteLine(result);
        // Hello! How can I assist you today?
        
        //// `inputKey` and `outputKey` here is more for understanding of what goes where. They have default values and can be omitted. Also there is classes like `Gpt35TurboModel` for simplicity.

        //// ## Additional options
        //// You can pass custom HttpClient/HttpClientHandler by using `OpenAiProvider` constructor overload.
        var httpClientHandler = new HttpClientHandler();
        using var httpClient = new HttpClient(httpClientHandler, disposeHandler: true);
        using var api = new OpenAiApi(
            httpClient,
            baseUri: new Uri("https://api.openai.com/v1/") // default value, can be omitted
            );
        var provider = new OpenAiProvider(api);
        var tunedModel = new OpenAiLatestFastChatModel(provider);
    }
}