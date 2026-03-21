using LangChain.Providers;
using LangChain.Providers.HuggingFace;

using var client = new HttpClient();
var provider = new HuggingFaceProvider(apiKey: string.Empty, client);
var model = new HuggingFaceChatModel(provider, id: "gpt2");

var response = await model.GenerateAsync("What would be a good company name be for name a company that makes colorful socks?");

Console.WriteLine("### GPT2 Response");
Console.WriteLine(response);

const string imageToTextModelId = "Salesforce/blip-image-captioning-base";
var imageModel = new HuggingFaceImageToTextModel(provider, imageToTextModelId);

var path = Path.Combine(Path.GetTempPath(), "solar_system.png");
var imageData = await File.ReadAllBytesAsync(path);
var binaryData = new BinaryData(imageData, "image/jpg");

var imageToTextResponse = await imageModel.GenerateTextFromImageAsync(new ImageToTextRequest
{
    Image = binaryData
});

Console.WriteLine("\n\n### ImageToText Response");
Console.WriteLine(imageToTextResponse.Text);

Console.ReadLine();
