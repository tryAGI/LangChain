using LangChain.Providers;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Providers.LLamaSharp;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task GettingStarted()
    {
        //// Ok, so you want to explore Large Language Models(LLM's) like ChatGPT with help of greatest programming language in the world(c#)?
        //// 
        //// `Scroll till the end of the page if you just want code`
        //// 
        //// Let's do this!
        //// 
        //// You, probably, heard that ChatGPT API costs some money, but don't worry. In this example we will use so called local models. You can think of it as a smaller version of ChatGPT that can run on your computer. **Even without graphics card!!!!** And completly free!
        //// 
        //// # Installation
        //// 
        //// Create new console application and use nuget to install these packages(dont't forget to check **_Include prerelease_**):
        //// 
        //// ```LangChain``` - meta package for development, it includes the Core package, OpenAI, HuggingFace and LLamaSharp providers
        //// 
        //// You also need to install ONE of these packages for the backend if you are using the LLamaSharp provider
        //// ```
        //// LLamaSharp.Backend.Cpu  # cpu for windows, linux and mac (mac metal is also supported)
        //// LLamaSharp.Backend.Cuda11  # cuda11 for windows and linux
        //// LLamaSharp.Backend.Cuda12  # cuda12 for windows and linux
        //// ```
        //// 
        //// # Preparing the model
        //// 
        //// Lets download our model first. For this example we will be using model called **Thespis** with 13 billions parameters and quantization Q2_K.
        //// 
        //// You can check it here: [Thespis-13B-v0.5-GGUF](https://huggingface.co/TheBloke/Thespis-13B-v0.5-GGUF?not-for-all-audiences=true)
        //// 
        //// You can look for other models here: [TheBloke](https://huggingface.co/TheBloke). We can only run models in GGUF format.
        //// 
        //// _What is quantization?_ - you may ask
        //// 
        //// In simple words in is precision of the model. Models with small quantization are easily getting confused with complex prompts, but are working much faster.
        //// 
        //// 
        //// ***
        //// 
        //// So, finally, let's write some code!
        
        // get model path
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");
        
        //// This line will download the model and save it locally for future usage. After model is downloaded it will return path to the *.gguf file. 
        //// _**You can manually download any model you want and insert path to it directly. Without using HuggingFaceModelDownloader.**_
        //// 
        //// Now it's time to load our model into memory:

        // load model
        var model = LLamaSharpModelInstruction.FromPath(modelPath).UseConsoleForDebug();
        
        //// Now let's build a chain!
        //// 
        //// # Building a chain
        //// 
        //// This is minimal chain to make LLM work:
        
        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(model, inputKey: "prompt");

        await chain.RunAsync();
        
        //// We can see here 2 chains(or links) working together: Set and LLM.
        //// 
        //// * Set - setting value for the _chain context variable **prompt**_
        //// * LLM - getting value from _chain context variable **prompt**_ and passing it to specified model
        //// 
        //// Links are connected together with '|' symbol.
        //// 
        //// Finally we run the entire chain. After some seconds you will see entire dialog in console window:
        //// 
        //// ```
        //// You are an AI assistant that greets the world.
        //// World: Hello, Assistant!
        //// Assistant: Hello, World! How can I help you today?
        //// ```
    }
}