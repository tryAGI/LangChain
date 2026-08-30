using Microsoft.Extensions.AI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    [Explicit("Requires OPENAI_API_KEY")]
    public async Task ImageGenerationWithOllamaAndStableDiffusion()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        // Using OpenAI via MEAI IChatClient (originally used Ollama — will migrate back when Ollama NuGet adds MEAI support)
        var openAiClient = new OpenAI.OpenAIClient(apiKey);
        IChatClient llm = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();

        var template =
    @"[INST]Transcript of a dialog, where the User interacts with an Assistant named Stablediffy. Stablediffy knows much about prompt engineering for stable diffusion (an open-source image generation software). The User asks Stablediffy about prompts for stable diffusion Image Generation.

Possible keywords for stable diffusion: ""cinematic, colorful background, concept art, dramatic lighting, high detail, highly detailed, hyper realistic, intricate, intricate sharp details, octane render, smooth, studio lighting, trending on artstation""
[/INST]
-- Transcript --

USER: suggest a prompt for a young girl from Swiss sitting by the window with headphones on
ASSISTANT: gorgeous young Swiss girl sitting by window with headphones on, octane render, unreal engine, photorealistic, hyper realism, highly detailed, 85mm portrait photography

USER: suggest a prompt for a mysterious city
ASSISTANT: Mysterious city, cityscape, urban, downtown, street, noir style, cinematic lightning, dramatic lightning, intricate, sharp details, octane render

USER:suggest a prompt for {value}
ASSISTANT:";

        // Generate a Stable Diffusion prompt using LLM
        var chain = Set("a cute girl cosplaying a cat")
                    | Template(template, outputKey: "prompt")
                    | LLM(llm, inputKey: "prompt", outputKey: "image_prompt");

        var result = await chain.RunAsync("image_prompt");
        Console.WriteLine($"Generated SD prompt: {result}");
    }
}
