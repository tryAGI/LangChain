using LangChain.Providers;
using LangChain.Providers.HuggingFace;
using LangChain.Providers.HuggingFace.Predefined;

using var client = new HttpClient();
var provider = new HuggingFaceProvider(apiKey: string.Empty, client);
var gpt2Model = new Gpt2Model(provider);

var gp2ModelResponse = await gpt2Model.GenerateAsync("What would be a good company name be for name a company that makes colorful socks?");

Console.WriteLine("### GP2 Response");
Console.WriteLine(gp2ModelResponse);

const string imageToTextModel = "Salesforce/blip-image-captioning-base";
var model = new HuggingFaceImageToTextModel(provider, imageToTextModel);

var path = Path.Combine(Path.GetTempPath(), "solar_system.png");
var imageData = await File.ReadAllBytesAsync(path);
var binaryData = new BinaryData(imageData, "image/jpg");

var imageToTextResponse = await model.GenerateTextFromImageAsync(new ImageToTextRequest
{
    Image = binaryData
});

Console.WriteLine("\n\n### ImageToText Response");
Console.WriteLine(imageToTextResponse.Text);

Console.ReadLine();
