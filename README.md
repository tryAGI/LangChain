# 🦜️🔗 LangChain .NET

[![Nuget package](https://img.shields.io/nuget/vpre/LangChain)](https://www.nuget.org/packages/LangChain/)
[![dotnet](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/tryAGI/LangChain)](https://github.com/tryAGI/LangChain/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1115206893015662663?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/Ca2xhfBf3v)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-16-orange.svg?style=flat-square)](#contributors-)
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
You can use our wiki to get started: https://github.com/tryAGI/LangChain/wiki  
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
var llm = new Gpt35TurboModel(provider);
var embeddingModel = new TextEmbeddingV3SmallModel(provider);

// Create vector database from Harry Potter book pdf
using var vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
    embeddingModel, // Used to convert text to embeddings
    dimensions: 1536, // Should be 1536 for TextEmbeddingV3SmallModel
    dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
    collectionName: "harry-potter", // Can be omitted, use if you want to have multiple collections
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
     """, cancellationToken: CancellationToken.None).ConfigureAwait(false);

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
var chainAnswer = await chain.RunAsync("text", CancellationToken.None);  // get chain result

Console.WriteLine("Chain Answer:"+ chainAnswer);       // print the result
        
Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price
Console.WriteLine($"Embedding model usage: {embeddingModel.Usage}");   // Print usage and price
```

## Featured projects
- [LangChainChat](https://github.com/TesAnti/LangChainChat) - Allows you to run a chat based on a Blazor project using LangChain.Serve and any of the supported local or paid models

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://www.upwork.com/freelancers/~017b1ad6f6af9cc189"><img src="https://avatars.githubusercontent.com/u/3002068?v=4?s=100" width="100px;" alt="Konstantin S."/><br /><sub><b>Konstantin S.</b></sub></a><br /><a href="#infra-HavenDV" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=HavenDV" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=HavenDV" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/TesAnti"><img src="https://avatars.githubusercontent.com/u/8780022?v=4?s=100" width="100px;" alt="TesAnti"/><br /><sub><b>TesAnti</b></sub></a><br /><a href="#infra-TesAnti" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=TesAnti" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=TesAnti" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/khoroshevj"><img src="https://avatars.githubusercontent.com/u/13628506?v=4?s=100" width="100px;" alt="Khoroshev Evgeniy"/><br /><sub><b>Khoroshev Evgeniy</b></sub></a><br /><a href="#infra-khoroshevj" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=khoroshevj" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=khoroshevj" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/SiegDuch"><img src="https://avatars.githubusercontent.com/u/104992451?v=4?s=100" width="100px;" alt="SiegDuch"/><br /><sub><b>SiegDuch</b></sub></a><br /><a href="#infra-SiegDuch" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/gunpal5"><img src="https://avatars.githubusercontent.com/u/10114874?v=4?s=100" width="100px;" alt="gunpal5"/><br /><sub><b>gunpal5</b></sub></a><br /><a href="#infra-gunpal5" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=gunpal5" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=gunpal5" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/kharedev247"><img src="https://avatars.githubusercontent.com/u/72281217?v=4?s=100" width="100px;" alt="Ketan Khare"/><br /><sub><b>Ketan Khare</b></sub></a><br /><a href="#infra-kharedev247" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=kharedev247" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=kharedev247" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://rooc.nl"><img src="https://avatars.githubusercontent.com/u/5981147?v=4?s=100" width="100px;" alt="Roderic Bos"/><br /><sub><b>Roderic Bos</b></sub></a><br /><a href="#infra-IRooc" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=IRooc" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=IRooc" title="Code">💻</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/hiptopjones"><img src="https://avatars.githubusercontent.com/u/3208743?v=4?s=100" width="100px;" alt="Peter James"/><br /><sub><b>Peter James</b></sub></a><br /><a href="#infra-hiptopjones" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=hiptopjones" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=hiptopjones" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/curlyfro"><img src="https://avatars.githubusercontent.com/u/127311?v=4?s=100" width="100px;" alt="Ty Augustine"/><br /><sub><b>Ty Augustine</b></sub></a><br /><a href="#infra-curlyfro" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=curlyfro" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=curlyfro" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/ericgreenmix"><img src="https://avatars.githubusercontent.com/u/1297049?v=4?s=100" width="100px;" alt="Eric Green"/><br /><sub><b>Eric Green</b></sub></a><br /><a href="#infra-ericgreenmix" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=ericgreenmix" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=ericgreenmix" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/Lyx52"><img src="https://avatars.githubusercontent.com/u/55701905?v=4?s=100" width="100px;" alt="Lyx52"/><br /><sub><b>Lyx52</b></sub></a><br /><a href="#infra-Lyx52" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=Lyx52" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=Lyx52" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/fiorelorenzo"><img src="https://avatars.githubusercontent.com/u/29930922?v=4?s=100" width="100px;" alt="Lorenzo Fiore"/><br /><sub><b>Lorenzo Fiore</b></sub></a><br /><a href="#infra-fiorelorenzo" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=fiorelorenzo" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=fiorelorenzo" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/matt-regier"><img src="https://avatars.githubusercontent.com/u/13575802?v=4?s=100" width="100px;" alt="matt-regier"/><br /><sub><b>matt-regier</b></sub></a><br /><a href="#infra-matt-regier" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=matt-regier" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=matt-regier" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/jekakmail"><img src="https://avatars.githubusercontent.com/u/1722792?v=4?s=100" width="100px;" alt="Sotski Eugene"/><br /><sub><b>Sotski Eugene</b></sub></a><br /><a href="https://github.com/tryAGI/LangChain/commits?author=jekakmail" title="Code">💻</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/vikhyat90"><img src="https://avatars.githubusercontent.com/u/9795857?v=4?s=100" width="100px;" alt="vikhyat90"/><br /><sub><b>vikhyat90</b></sub></a><br /><a href="#infra-vikhyat90" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=vikhyat90" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=vikhyat90" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/ceejeeb"><img src="https://avatars.githubusercontent.com/u/11246686?v=4?s=100" width="100px;" alt="ceejeeb"/><br /><sub><b>ceejeeb</b></sub></a><br /><a href="#infra-ceejeeb" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=ceejeeb" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=ceejeeb" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

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