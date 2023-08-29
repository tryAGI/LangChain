using LangChain.Providers;
using LangChain.Providers.HuggingFace;

using var client = new HttpClient();
var gpt2Model = new Gpt2Model(apiKey: string.Empty, client);

var response = await gpt2Model.GenerateAsync("What would be a good company name be for name a company that makes colorful socks?");

Console.WriteLine(response);