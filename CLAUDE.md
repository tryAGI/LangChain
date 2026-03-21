# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

C# implementation of the LangChain framework for building applications with LLMs through composability. Provides chains, memory, RAG (Retrieval-Augmented Generation), document loaders, text splitters, and a serving layer. Distributed as multiple NuGet packages under the `LangChain` namespace. The meta-package `LangChain` bundles the most commonly used providers (OpenAI, Anthropic, Google, Ollama, Azure, etc.) with the core library.

## Build and Test Commands

```bash
# Build the entire solution
dotnet build LangChain.slnx

# Run all integration tests
dotnet test src/Meta/test/LangChain.IntegrationTests.csproj

# Run core unit tests
dotnet test src/Core/test/UnitTests/LangChain.Core.UnitTests.csproj

# Run splitter tests
dotnet test src/Splitters/Abstractions/test/LangChain.Splitters.Abstractions.Tests.csproj

# Run a specific test
dotnet test src/Meta/test/LangChain.IntegrationTests.csproj --filter "FullyQualifiedName~WikiTests"

# Validate trimming/NativeAOT compatibility (requires: dotnet tool install -g autosdk.cli --prerelease)
autosdk trim src/libs/*//*.csproj
```

Integration tests require API keys via environment variables (e.g., `OPENAI_API_KEY`). Tests skip (not fail) if keys are unset.

## Architecture

### Project Structure

```
src/
├── Core/src/                  # LangChain.Core — chains, memory, prompts, retrievers, schema
├── Meta/src/                  # LangChain (meta-package) — bundles Core + popular providers
├── Meta/test/                 # Integration tests (WikiTests, ReadmeTests)
├── DocumentLoaders/
│   ├── Abstractions/          # LangChain.DocumentLoaders.Abstractions
│   ├── Pdf/                   # LangChain.DocumentLoaders.Pdf (PdfPig-based)
│   ├── WebBase/               # LangChain.DocumentLoaders.Html
│   └── Word/                  # LangChain.DocumentLoaders.Word
├── Splitters/
│   ├── Abstractions/          # LangChain.Splitters.Abstractions (CharacterTextSplitter, etc.)
│   └── CSharp/                # LangChain.Splitters.CSharp
├── Extensions/
│   ├── DependencyInjection/   # LangChain.Extensions.DependencyInjection
│   └── Docker/                # LangChain.Extensions.Docker
├── Serve/
│   ├── Abstractions/          # LangChain.Serve.Abstractions
│   ├── src/                   # LangChain.Serve (ASP.NET Core middleware)
│   └── OpenAI/                # LangChain.Serve.OpenAI (OpenAI-compatible serving)
├── Utilities/
│   ├── Pollyfils/             # LangChain.Polyfills (framework polyfills)
│   ├── Sql/                   # LangChain.Utilities.Sql
│   └── Postgres/              # LangChain.Utilities.Postgres
├── Cli/                       # LangChain.Cli
└── Providers/Abstractions/    # Local copy of provider abstractions (mirrors LangChain.Providers)
examples/                      # Sample projects (OpenAI, Azure, Memory, Serve, LocalRAG, etc.)
```

### Core Abstractions (src/Core/src/)

**Chains** (`Chains/`):
- `Chain` static class — fluent API for composing stackable chains using the `|` operator
- Key chain factory methods: `Set()`, `Template()`, `LLM()`, `RetrieveSimilarDocuments()`, `CombineDocuments()`, `LoadMemory()`, `UpdateMemory()`, `TTS()`, `STT()`, `ReActAgentExecutor()`, `GroupChat()`
- `StackableChains/` — individual chain implementations (LLM, ReAct agents, Crew agents, file chains, image generation)
- Chains are composed via the pipe `|` operator and executed with `chain.RunAsync("key")`

**Memory** (`Memory/`):
- `ConversationBufferMemory` — stores full conversation history
- `ConversationSummaryMemory` — stores summarized conversation history
- `ConversationSummaryBufferMemory` — hybrid (recent messages + summarized history)
- `ConversationWindowBufferMemory` — sliding window of recent messages

**Prompts** (`Prompts/`):
- Prompt template system with variable substitution via `{variable_name}` placeholders

**Base Classes** (`Base/`):
- `BaseChain` — abstract base for all chains
- `BaseCallbackHandler` — callback system for tracing and debugging

### Dependencies

LangChain.Core depends on:
- `LangChain.Providers.Abstractions` (NuGet) — non-MEAI provider interfaces (ITextToSpeechModel, ITextToImageModel, IImageToTextModel) and message types (Message, MessageRole)
- `LangChain.Databases.Abstractions` (NuGet) — message history (BaseChatMessageHistory, ChatMessageHistory)
- `Microsoft.Extensions.AI` (NuGet) — MEAI interfaces (IChatClient, IEmbeddingGenerator, ISpeechToTextClient)
- `Microsoft.Extensions.VectorData.Abstractions` (NuGet) — vector store abstractions
- `LangChain.DocumentLoaders.Abstractions` (project reference)
- `LangChain.Splitters.Abstractions` (project reference)

The meta-package additionally references:
- `tryAGI.OpenAI`, `tryAGI.Anthropic`, `Ollama` — SDK packages implementing MEAI interfaces natively
- `Google_Gemini` — conditional on net10.0 (only TFM it supports)
- `Microsoft.SemanticKernel.Connectors.InMemory` — in-memory vector store

### Key Patterns

**Chain Composition** — the primary pattern for building LLM workflows:
```csharp
var chain =
    Set("question text")
    | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5)
    | CombineDocuments(outputKey: "context")
    | Template(promptTemplate)
    | LLM(llm);
var result = await chain.RunAsync("text");
```

**MEAI Provider Pattern** — use tryAGI SDKs directly with MEAI interfaces:
```csharp
var openAiClient = new OpenAIClient(apiKey);
IChatClient llm = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embeddingModel = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();
```

**RAG Pattern** — load documents, create embeddings, query with similarity search:
```csharp
var vectorStore = new InMemoryVectorStore();
var vectorCollection = await vectorStore.AddDocumentsFromAsync<PdfPigPdfLoader>(
    embeddingModel, dimensions: 1536, dataSource: DataSource.FromUrl(url));
var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question);
```

## Key Conventions

- **Target frameworks:** `net4.6.2`, `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`
- **Language:** C# preview, nullable reference types enabled, implicit usings
- **Strong naming:** All assemblies signed with `src/key.snk`
- **Versioning:** MinVer with `v` tag prefix
- **Testing:** MSTest framework
- **Central package management:** `src/Directory.Packages.props`
- Cross-project dependencies between LangChain ecosystem repos are via NuGet packages, not project references
