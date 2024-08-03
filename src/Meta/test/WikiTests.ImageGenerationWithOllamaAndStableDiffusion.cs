using LangChain.Providers;
using LangChain.Providers.Automatic1111;
using LangChain.Providers.Ollama;
using Ollama;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task ImageGenerationWithOllamaAndStableDiffusion()
    {
        var provider = new OllamaProvider(
            options: new RequestOptions
            {
                Stop = new[] { "\n" },
                Temperature = 0
            });
        var llm = new OllamaChatModel(provider, id: "mistral:latest").UseConsoleForDebug();

        var sdmodel = new Automatic1111Model
        {
            Settings = new Automatic1111ModelSettings
            {
                NegativePrompt = "bad quality, blured, watermark, text, naked, nsfw",
                Seed = 42, // for results repeatability
                CfgScale = 6.0f,
                Width = 512,
                Height = 768,
            },
        };

        var template =
    @"[INST]Transcript of a dialog, where the User interacts with an Assistant named Stablediffy. Stablediffy knows much about prompt engineering for stable diffusion (an open-source image generation software). The User asks Stablediffy about prompts for stable diffusion Image Generation. 

Possible keywords for stable diffusion: ""cinematic, colorful background, concept art, dramatic lighting, high detail, highly detailed, hyper realistic, intricate, intricate sharp details, octane render, smooth, studio lighting, trending on artstation, landscape, scenery, cityscape, underwater, salt flat, tundra, jungle, desert mountain, ocean, beach, lake, waterfall, ripples, swirl, waves, avenue, horizon, pasture, plateau, garden, fields, floating island, forest, cloud forest, grasslands, flower field, flower ocean, volcano, cliff, snowy mountain
city, cityscape, street, downtown""
[/INST]
-- Transcript --

USER: suggest a prompt for a young girl from Swiss sitting by the window with headphones on
ASSISTANT: gorgeous young Swiss girl sitting by window with headphones on, wearing white bra with translucent shirt over, soft lips, beach blonde hair, octane render, unreal engine, photograph, realistic skin texture, photorealistic, hyper realism, highly detailed, 85mm portrait photography, award winning, hard rim lighting photography

USER: suggest a prompt for an mysterious city
ASSISTANT: Mysterious city, cityscape, urban, downtown, street, noir style, cinematic lightning, dramatic lightning, intricate, sharp details, octane render, unreal engine, highly detailed, night scene, dark lighting, gritty atmosphere

USER: suggest a prompt for a high quality render of a car in 1950
ASSISTANT: Car in 1950, highly detailed, classic car, 1950's, highly detailed, dramatic lightning, cinematic lightning, unreal engine

USER:suggest a prompt for {value}
ASSISTANT:";


        var chain = Set("a cute girl cosplaying a cat")                                     // describe a desired image in simple words
                    | Template(template, outputKey: "prompt")                               // insert our description into the template
                    | LLM(llm, inputKey: "prompt", outputKey: "image_prompt")           // ask ollama to generate a prompt for stable diffusion
                    | GenerateImage(sdmodel, inputKey: "image_prompt", outputKey: "image")  // generate an image using stable diffusion
                    | SaveIntoFile("image.png", inputKey: "image");                     // save the image into a file

        // run the chain
        await chain.RunAsync();
    }
}