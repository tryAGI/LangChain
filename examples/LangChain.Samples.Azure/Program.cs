using LangChain.Providers;
using LangChain.Providers.Azure;

var model = new AzureOpenAIModel("AZURE_OPEN_AI_KEY", "ENDPOINT", "DEPLOYMENT_NAME");

var result = await model.GenerateAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result);
