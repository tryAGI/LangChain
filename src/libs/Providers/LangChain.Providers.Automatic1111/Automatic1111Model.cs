using LangChain.Providers;

namespace StableDiffusion;

/// <summary>
/// 
/// </summary>
public class Automatic1111Model : IGenerateImageModel
{
    /// <summary>
    /// 
    /// </summary>
    public Automatic1111ModelOptions Options { get; }
    
    private readonly StableDiffusionClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="options"></param>
    public Automatic1111Model(
        string url = "http://localhost:7860/",
        Automatic1111ModelOptions? options = null)
    {
        Options = options?? new Automatic1111ModelOptions();
        HttpClient httpClient = new HttpClient();
        _client = new StableDiffusionClient(url, httpClient);
    }

    public event Action<string> PromptSent = delegate { };

    /// <inheritdoc />
    public Task<Uri> GenerateImageAsUrlAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return Task.FromException<Uri>(new NotSupportedException());
    }

    /// <inheritdoc />
    public async Task<Stream> GenerateImageAsStreamAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var bytes = await GenerateImageAsBytesAsync(prompt, cancellationToken).ConfigureAwait(false);
        return new MemoryStream(bytes);
    }

    /// <inheritdoc />
    public async Task<byte[]> GenerateImageAsBytesAsync(string prompt, CancellationToken cancellationToken = default)
    {
        PromptSent(prompt);

        var samplers = await _client.Get_samplers_sdapi_v1_samplers_getAsync();
        var response = await _client.Text2imgapi_sdapi_v1_txt2img_postAsync(
            new StableDiffusionProcessingTxt2Img
            {
                Prompt = prompt,
                Negative_prompt = Options.NegativePrompt,
                Height = Options.Height,
                Width = Options.Width,
                Steps = Options.Steps,
                Seed = Options.Seed,
                Cfg_scale = Options.CfgScale,
                Sampler_index = Options.Sampler,
                Sampler_name = Options.Sampler,
            }, cancellationToken).ConfigureAwait(false);

        var encoded = response.Images.First();
        // base64 to png

        var bytes = Convert.FromBase64String(encoded);
        return bytes;
    }
}
