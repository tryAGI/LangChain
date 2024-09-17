# 🦜️🔗 LangChain .NET

[![Nuget package](https://img.shields.io/nuget/vpre/LangChain)](https://www.nuget.org/packages/LangChain/)
[![dotnet](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/tryAGI/LangChain)](https://github.com/tryAGI/LangChain/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1115206893015662663?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/Ca2xhfBf3v)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-17-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

⚡ Building applications with LLMs through composability ⚡  
C# implementation of LangChain. We try to be as close to the original as possible in terms of abstractions, but are open to new entities.

While the [SemanticKernel](https://github.com/microsoft/semantic-kernel/) is good and we will use it wherever possible, we believe that it has many limitations and based on Microsoft technologies.
We proceed from the position of the maximum choice of available options and are open to using third-party libraries within individual implementations.  

I want to note:
- I’m unlikely to be able to make serious progress alone, so my goal is to unite the efforts of C# developers to create a C# version of LangChain and control the quality of the final project
- I try to accept any Pull Request within 24 hours (of course, it depends, but I will try)
- I'm also looking for developers to join the core team. I will sponsor them whenever possible and also share any money received.
- I also respond quite quickly on Discord for any questions related to the project

## Usage
You can use our wiki to get started: https://tryagi.github.io/LangChain/  
If the wiki contains unupdated code, you can always take a look at [the tests for this](src/Meta/test/WikiTests.cs)  
Also see [examples](./examples) for example usage or [tests](./src/tests/LangChain.IntegrationTests/ReadmeTests.cs).
```csharp
// Price to run from zero(create embeddings and request to LLM): 0,015$
// Price to re-run if database is exists: 0,0004$
// Dependencies: LangChain, LangChain.Databases.Sqlite, LangChain.DocumentLoaders.Pdf

// Initialize models
var provider = new OpenAiProvider(
    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
    throw new InconclusiveException("OPENAI_API_KEY is not set"));
var llm = new OpenAiLatestFastChatModel(provider);
var embeddingModel = new TextEmbeddingV3SmallModel(provider);

// Create vector database from Harry Potter book pdf
using var vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
    embeddingModel, // Used to convert text to embeddings
    dimensions: 1536, // Should be 1536 for TextEmbeddingV3SmallModel
    dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
    collectionName: "harrypotter", // Can be omitted, use if you want to have multiple collections
    textSplitter: null); // Default is CharacterTextSplitter(ChunkSize = 4000, ChunkOverlap = 200)

// Now we have two ways: use the async methods or use the chains
// 1. Async methods

// Find similar documents for the question
const string question = "Who was drinking a unicorn blood?";
var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);

// Use similar documents and LLM to answer the question
var answer = await llm.GenerateAsync(
    $"""
     Use the following pieces of context to answer the question at the end.
     If the answer is not in context then just say that you don't know, don't try to make up an answer.
     Keep the answer as short as possible.

     {similarDocuments.AsString()}

     Question: {question}
     Helpful Answer:
     """);

Console.WriteLine($"LLM answer: {answer}"); // The cloaked figure.

// 2. Chains
var promptTemplate =
    @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
{context}
Question: {text}
Helpful Answer:";

var chain =
    Set("Who was drinking a unicorn blood?")     // set the question (default key is "text")
    | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5) // take 5 most similar documents
    | CombineDocuments(outputKey: "context")     // combine documents together and put them into context
    | Template(promptTemplate)                   // replace context and question in the prompt with their values
    | LLM(llm.UseConsoleForDebug());             // send the result to the language model
var chainAnswer = await chain.RunAsync("text");  // get chain result

Console.WriteLine("Chain Answer:"+ chainAnswer);       // print the result
        
Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price
Console.WriteLine($"Embedding model usage: {embeddingModel.Usage}");   // Print usage and price
```

## Featured projects
- [LangChainChat](https://github.com/TesAnti/LangChainChat) - Allows you to run a chat based on a Blazor project using LangChain.Serve and any of the supported local or paid models

## 🌟 Contributors

[![langchain.net contributors](https://contrib.rocks/image?repo=tryagi/langchain&max=2000)](https://github.com/tryagi/langchain/graphs/contributors)

## Support

Priority place for bugs: https://github.com/tryAGI/LangChain/issues  
Priority place for ideas and general questions: https://github.com/tryAGI/LangChain/discussions  
Discord: https://discord.gg/Ca2xhfBf3v  

## Legal information and credits
It's licensed under [the MIT license](LICENSE). We do not plan to change the license in any foreseeable future for this project, 
but projects based on this within the organization may have different licenses.  
Some documentation is based on documentation from [dotnet/docs](https://github.com/dotnet/docs/) repository 
under [CC BY 4.0 license](https://github.com/dotnet/docs/blob/main/LICENSE), 
where code examples are changed to code examples for using this project.  

## Acknowledgments

![JetBrains logo](https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.png)

This project is supported by JetBrains through the [Open Source Support Program](https://jb.gg/OpenSourceSupport).

![CodeRabbit logo](https://opengraph.githubassets.com/1c51002d7d0bbe0c4fd72ff8f2e58192702f73a7037102f77e4dbb98ac00ea8f/marketplace/coderabbitai)

This project is supported by CodeRabbit through the [Open Source Support Program](https://github.com/marketplace/coderabbitai).