using LangChain.Providers.Azure;

var provider = new AzureOpenAiProvider(apiKey: "AZURE_OPEN_AI_KEY", endpoint: "ENDPOINT");
var llm = new AzureOpenAiChatModel(provider, id: "DEPLOYMENT_NAME");

var result = await llm.GenerateAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result);

//Image test
//var imageUrl = await model.GenerateImageAsUrlAsync("a blue horse called azure");
//Console.WriteLine(model.RevisedPromptResult);
//Console.WriteLine(imageUrl);
