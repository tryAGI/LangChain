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
    # Note: Non-MEAI provider interfaces (TTS, Image, ImageToText) are in Core/src/Providers/
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
- `LangChain.Databases.Abstractions` (NuGet) — message history (BaseChatMessageHistory, ChatMessageHistory)
- `Microsoft.Extensions.AI` (NuGet) — MEAI interfaces (IChatClient, IEmbeddingGenerator, ISpeechToTextClient, ChatMessage, ChatRole)
- `Microsoft.Extensions.VectorData.Abstractions` (NuGet) — MEVA vector store abstractions (`VectorStore`, `VectorStoreCollection<TKey, TRecord>`)
- `LangChain.DocumentLoaders.Abstractions` (project reference)
- `LangChain.Splitters.Abstractions` (project reference)
- Non-MEAI provider interfaces (ITextToSpeechModel, ITextToImageModel, IImageToTextModel) are defined locally in `src/Core/src/Providers/`

The meta-package additionally references:
- `tryAGI.OpenAI`, `tryAGI.Anthropic`, `Ollama` — SDK packages implementing MEAI interfaces natively
- `Google_Gemini` — conditional on net10.0 (only TFM it supports)
- `Microsoft.SemanticKernel.Connectors.InMemory` — in-memory MEVA vector store

### Vector Store (MEVA) Architecture

The project uses **Microsoft.Extensions.VectorData.Abstractions (MEVA)** for all vector store operations. The old `IVectorDatabase`/`IVectorCollection` interfaces from `LangChain.Databases` have been replaced.

**Key types:**
- `VectorStore` — abstract base for vector store providers (InMemory, SqliteVec, OpenSearch, etc.)
- `VectorStoreCollection<string, LangChainDocumentRecord>` — typed collection for document storage and search
- `LangChainDocumentRecord` (`src/Core/src/Schema/LangChainDocumentRecord.cs`) — MEVA-attributed record bridging LangChain's `Document` to MEVA:
  - `Id` (string, `[VectorStoreKey]`) — auto-generated GUID
  - `Text` (string?, `[VectorStoreData]`) — document content
  - `MetadataJson` (string?, `[VectorStoreData]`) — JSON-serialized metadata dictionary
  - `Embedding` (ReadOnlyMemory\<float\>, `[VectorStoreVector(1536)]`) — vector embedding

**Extension methods** (`src/Core/src/Extensions/`):
- `VectorCollectionExtensions` — `GetSimilarDocuments()`, `AddDocumentsAsync()`, `AddTextsAsync()`, `GetDocumentByIdAsync()`, `AsString()` on `VectorStoreCollection<string, LangChainDocumentRecord>`
- `VectorDatabaseExtensions` — `AddDocumentsFromAsync<TLoader>()` on `VectorStore` (creates collection with runtime dimensions via `VectorStoreCollectionDefinition`)

**Vector store providers used:**
- `Microsoft.SemanticKernel.Connectors.InMemory` — in-memory (for tests and simple usage)
- `Microsoft.SemanticKernel.Connectors.SqliteVec` — SQLite-based persistent storage
- `LangChain.Databases.OpenSearch` — OpenSearch (uses MEVA v0.17.1+)

### Key Patterns

**Chain Composition** — the primary pattern for building LLM workflows:
```csharp
var chain =
    Set("question text")
    | RetrieveSimilarDocuments(vectorCollection, embeddingGenerator, amount: 5)
    | CombineDocuments(outputKey: "context")
    | Template(promptTemplate)
    | LLM(chatClient);
var result = await chain.RunAsync("text");
```

**MEAI Provider Pattern** — use tryAGI SDKs directly with MEAI interfaces:
```csharp
var openAiClient = new OpenAIClient(apiKey);
IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();
```

**RAG Pattern** — load documents, create embeddings, query with similarity search (using MEVA):
```csharp
// Option 1: Convenience method — creates collection + loads documents in one call
var vectorStore = new InMemoryVectorStore();
var vectorCollection = await vectorStore.AddDocumentsFromAsync<PdfPigPdfLoader>(
    embeddingGenerator, dimensions: 1536, dataSource: DataSource.FromUrl(url));
var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingGenerator, question);

// Option 2: Manual — create collection, add documents, then search
var store = new InMemoryVectorStore();
var collection = store.GetCollection<string, LangChainDocumentRecord>("my-collection");
await collection.EnsureCollectionExistsAsync();
await collection.AddDocumentsAsync(embeddingGenerator, documents);
var results = await collection.GetSimilarDocuments(embeddingGenerator, question, amount: 5);
```

## Key Conventions

- **Target frameworks:** `net4.6.2`, `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`
- **Language:** C# preview, nullable reference types enabled, implicit usings
- **Strong naming:** All assemblies signed with `src/key.snk`
- **Versioning:** MinVer with `v` tag prefix
- **Testing:** NUnit framework with AwesomeAssertions
- **Central package management:** `src/Directory.Packages.props`
- Cross-project dependencies between LangChain ecosystem repos are via NuGet packages, not project references
