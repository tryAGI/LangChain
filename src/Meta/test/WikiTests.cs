using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Memory;
using LangChain.Providers;
using LangChain.Providers.Automatic1111;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Providers.LLamaSharp;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Sources;
using LangChain.Splitters.Text;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class WikiTests
{
    [Test]
    public async Task AgentWithOllama()
    {
        var model = new OllamaLanguageModelInstruction("mistral:latest",
            "http://localhost:11434",
            options: new OllamaLanguageModelOptions
            {
                Temperature = 0,
            }).UseConsoleForDebug();

        var chain =
            Set("What is tryAGI/LangChain?")
            | LLM(model);

        await chain.RunAsync();
    }

    [Test]
    public async Task AgentWithOllamaReact()
    {
        var model = new OllamaLanguageModelInstruction("mistral:latest",
            "http://localhost:11434",
            options: new OllamaLanguageModelOptions()
            {
                Stop = new[] { "Observation", "[END]" }, // add injection word `Observation` and `[END]` to stop the model(just as additional safety feature)
                Temperature = 0
            }).UseConsoleForDebug();

        // create a google search tool
        var searchTool = new GoogleCustomSearchTool(key: "<your key>", cx: "<your cx>", resultsLimit: 1);

        var chain =
            Set("What is tryAGI/LangChain?")
            | ReActAgentExecutor(model) // does the magic
                .UseTool(searchTool); // add the google search tool

        await chain.RunAsync();
    }

    [Test]
    public async Task BuildingChatWithOpenAi()
    {
        // we will use GPT-3.5 model, but you can use any other model
        var model = new Gpt35TurboModel("your_key");


        // create simple template for conversation for AI to know what piece of text it is looking at
        var template =
            @"The following is a friendly conversation between a human and an AI.
{history}
Human: {input}
AI:";


        // To have a conversation thar remembers previous messages we need to use memory.
        // For memory to work properly we need to specify AI and Human prefixes.
        // Since in our template we have "AI:" and "Human:" we need to specify them here. Pay attention to spaces after prefixes.
        var conversationBufferMemory = new ConversationBufferMemory(new ChatMessageHistory());// TODO: Review { AiPrefix = "AI: ", HumanPrefix = "Human: "};

        // build chain. Notice that we don't set input key here. It will be set in the loop
        var chain =
            // load history. at first it will be empty, but UpdateMemory will update it every iteration
            LoadMemory(conversationBufferMemory, outputKey: "history")
            | Template(template)
            | LLM(model)
            // update memory with new request from Human and response from AI
            | UpdateMemory(conversationBufferMemory, requestKey: "input", responseKey: "text");

        // run an endless loop of conversation
        while (true)
        {
            Console.Write("Human: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (input == "exit")
                break;

            // build a new chain using previous chain but with new input every time
            var chatChain = Set(input, "input")
                            | chain;

            // get response from AI
            var res = await chatChain.RunAsync("text", CancellationToken.None);


            Console.Write("AI: ");
            Console.WriteLine(res);
        }
    }

    [Test]
    public async Task GettingStarted()
    {
        // get model path
        var modelPath = await HuggingFaceModelDownloader.Instance.GetModel(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");

        // load model
        var model = LLamaSharpModelInstruction.FromPath(modelPath).UseConsoleForDebug();

        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(model, inputKey: "prompt");

        await chain.RunAsync();
    }

    [Test]
    public async Task HowToUseOpenAiProvider()
    {
        var model = new Gpt35TurboModel("your_openAI_key");
        var chain =
            Set("Hello!", outputKey: "request")          // set context variable `request` to "Hello"
            | LLM(model, inputKey: "request", outputKey: "text"); // get text from context variable `request`, pass it to the model and put result into `text`

        var result = await chain.RunAsync("text", CancellationToken.None);  // execute chain and get `text` context variable
        Console.WriteLine(result);
    }

    [Test]
    public async Task HowToUseOpenAiProviderSmaller()
    {
        var model = new Gpt35TurboModel("your_openAI_key");
        var chain =
            Set("Hello!")
            | LLM(model);

        Console.WriteLine(await chain.RunAsync("text", CancellationToken.None));
    }

    [Test]
    public async Task ImageGenerationWithOllamaAndStableDiffusion()
    {
        var olmodel = new OllamaLanguageModelInstruction("mistral:latest",
            "http://localhost:11434",
            options: new OllamaLanguageModelOptions()
            {
                Stop = new[] { "\n" },
                Temperature = 0
            }).UseConsoleForDebug();

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
                    | LLM(olmodel, inputKey: "prompt", outputKey: "image_prompt")           // ask ollama to generate a prompt for stable diffusion
                    | GenerateImage(sdmodel, inputKey: "image_prompt", outputKey: "image")  // generate an image using stable diffusion
                    | SaveIntoFile("image.png", inputKey: "image");                     // save the image into a file

        // run the chain
        await chain.RunAsync();
    }

    [Test]
    public async Task RagWithOpenAiOllama()
    {
        // prepare OpenAI embedding model
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OpenAI API key is not set");
        var embeddingModel = new TextEmbeddingV3SmallModel(apiKey);

        // prepare Ollama with mistral model
        var model = new OllamaLanguageModelInstruction("mistral:latest", options: new OllamaLanguageModelOptions
        {
            Stop = ["\n"],
            Temperature = 0.0f,
        }).UseConsoleForDebug();


        var vectorDatabase = new SqLiteVectorDatabase("vectors.db");
        var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync("harry-potter", dimensions: 1536);
        if (await vectorCollection.IsEmptyAsync())
        {
            var pdfSource = new PdfPigPdfSource("E:\\AI\\Datasets\\Books\\Harry-Potter-Book-1.pdf");
            var documents = await pdfSource.LoadAsync();

            await vectorCollection.AddSplitDocumentsAsync(
                embeddingModel,
                documents,
                textSplitter: new RecursiveCharacterTextSplitter(
                    chunkSize: 200,
                    chunkOverlap: 50));
        }

        string promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";


        var chain =
            Set("Who was drinking a unicorn blood?", outputKey: "question")                // set the question
            | RetrieveDocuments(vectorCollection, embeddingModel, inputKey: "question", outputKey: "documents", amount: 5) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(model);                                                                       // send the result to the language model

        var result = await chain.RunAsync("text", CancellationToken.None);                                        // get chain result

        Console.WriteLine(result);
    }

    [Test]
    public async Task UsingChainOutput()
    {
        // get model path
        var modelPath = await HuggingFaceModelDownloader.Instance.GetModel(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");

        // load model
        var model = LLamaSharpModelInstruction.FromPath(modelPath);

        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "result");

        var result = await chain.RunAsync("result", CancellationToken.None);
        Console.WriteLine("---");
        Console.WriteLine(result);
        Console.WriteLine("---");
    }
}