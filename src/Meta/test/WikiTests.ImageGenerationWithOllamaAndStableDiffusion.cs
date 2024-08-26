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
        //// # Prerequisites
        //// For this tutorial you would need Ollama and AUTOMATIC1111 locally installed. You can easily find instructions online how to do it for your system.
        //// I'm using Docker images for both of those.
        //// Also you would need about 32GB of RAM installed in your PC since two models are "eating" a lot of it.
        //// 
        //// # The problem
        //// Making a prompt for image generation models like Stable Diffusion is not a simple task. Instead of just asking what you need in simple sentence, you would need to describe all the small details about object and environment using quite big set of keywords.
        //// 
        //// Let's try to solve this problem!
        //// 
        //// # Setup
        //// 
        //// Create new console app and add nuget packages:
        //// ```
        //// LangChain.Core
        //// LangChain.Providers.Automatic1111
        //// LangChain.Providers.Ollama
        //// ```
        //// Now we are ready to code!
        //// 
        //// # Creating models
        //// 
        //// ## Ollama model
        //// We will use latest version of `llama3.1` for our task. If you don't have mistral yet - it will be downloaded.

        var provider = new OllamaProvider(
            options: new RequestOptions
            {
                Temperature = 0,
            });
        var llm = new OllamaChatModel(provider, id: "llama3.1").UseConsoleForDebug();

        //// Here we are stopping generation after `\n` symbol appears. Mistral will put a new line(`\n`) symbol after prompt is generated.
        //// We are using Temperature of 0 to always have the same result for the same prompt.
        //// assigning events to model is a good way to see what is going on.
        //// 
        //// ## Stable diffusion model
        //// 
        //// You can select your model in Automatic1111 UI.

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

        //// You should be familiar with these parameters if you were using SD before. But in simple words we're asking SD to generate a portrait without anything bad on it.
        //// 
        //// # Prompt
        //// At this point we are ready to start our LLM magic.  
        //// We will be using a [special prompting technique](https://www.promptingguide.ai/techniques/fewshot) to explain our expectations to mistral.  
        //// I took it from [here](https://github.com/vicuna-tools/Stablediffy/blob/main/Stablediffy.txt) with some minor modifications.  
        //// Basically, we are showing some examples so model could understand a principle of prompt generation. You can play around with examples and instructions to better match your preferences.  
        //// Now let's build a chain!  

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

        //// If everything done correctly - you should have `image.png` in your bin directory.
    }
}