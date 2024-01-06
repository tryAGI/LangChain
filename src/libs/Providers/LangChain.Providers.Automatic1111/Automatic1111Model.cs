﻿using LangChain.Providers;

namespace StableDiffusion;

public class Automatic1111Model: IGenerateImageModel
{
    public Automatic1111ModelOptions Options { get; }
    private readonly StableDiffusionClient _client;

    public Automatic1111Model(string url= "http://localhost:7860/", Automatic1111ModelOptions options=null )
    {
        Options = options?? new Automatic1111ModelOptions();
        HttpClient httpClient = new HttpClient();
        _client = new StableDiffusionClient(url, httpClient);
    }


    public async Task<Uri> GenerateImageAsUrlAsync(string prompt, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<Stream> GenerateImageAsStreamAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var bytes = await GenerateImageAsBytesAsync(prompt, cancellationToken);
        return new MemoryStream(bytes);
    }

    public async Task<byte[]> GenerateImageAsBytesAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _client.Text2imgapi_sdapi_v1_txt2img_postAsync(new StableDiffusionProcessingTxt2Img()
        {
            Prompt = prompt,
            Negative_prompt = Options.NegativePrompt,
            Height = Options.Height,
            Width = Options.Width,
            Steps = Options.Steps,
            Seed = Options.Seed,
            Cfg_scale = Options.CFGScale

        });

        var encoded = response.Images.First();
        // base64 to png

        var bytes = Convert.FromBase64String(encoded);
        return bytes;
    }
}