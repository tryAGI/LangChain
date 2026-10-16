# Local Chroma RAG sample

This .NET 10 sample uses Ollama on `localhost:11434` and Chroma on `localhost:8000`. It follows the existing LocalRAG sample and needs the `llama3` and `all-minilm` Ollama models. Start Chroma with `docker run --rm -p 127.0.0.1:8000:8000 chromadb/chroma:1.5.9`, then run the project. The PDF URL in Program.cs is downloaded when the sample is executed; replace it with a document you are authorized to use.

The Chroma adapter is `CommunityToolkit.VectorData.Chroma` 1.0.1-preview, which uses `ChromaDotNet.Client` 2.14.0 and Newtonsoft.Json. These are community dependencies in LangChain's documented integration exception, not first-party generated SDK runtime dependencies. The PDF and meta-package project references bring additional existing dependencies, including Aspose.PDF/PdfPig and provider clients. See the workspace SDK dependency policy before reusing this dependency graph in a generated SDK.

Validation on 2026-10-16: Release build; 40 core and 8 splitter tests; complete resolved sample graph vulnerability check with no reported vulnerable packages after updating the PDF loader's transitive Pkcs selection to 9.0.21. An isolated local Chroma 1.5.9 check used synthetic documents and deterministic three-dimensional embeddings to validate upsert, read-back and similarity search. It did not download this sample's PDF or run the Ollama models, so it is not full sample E2E.

The sample targets the normal .NET runtime. A NativeAOT publish completes with IL2026/IL3050 diagnostics in existing LangChain vector mapping and metadata serialization. NativeAOT/trimming compatibility is not established; do not suppress those diagnostics or claim a successful native runtime check from publication alone.
