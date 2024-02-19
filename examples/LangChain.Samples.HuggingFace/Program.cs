using LangChain.Providers.HuggingFace;
using LangChain.Providers.HuggingFace.Predefined;

using var client = new HttpClient();
var provider = new HuggingFaceProvider(apiKey: string.Empty, client);
var gpt2Model = new Gpt2Model(provider);

var response = await gpt2Model.GenerateAsync("What would be a good company name be for name a company that makes colorful socks?");

Console.WriteLine(response);