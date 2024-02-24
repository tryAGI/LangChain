using LangChain.Providers.Ollama;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public class OllamaLanguageModelOptions : OllamaOptions;

public class OllamaLanguageModelInstruction(
    string modelName,
    string url = "http://localhost:11434",
    OllamaLanguageModelOptions? options = null)
    : OllamaChatModel(new OllamaProvider(url, options), id: modelName);

public class OllamaLanguageModelEmbeddings(
    string modelName,
    string url = "http://localhost:11434",
    OllamaLanguageModelOptions? options = null)
    : OllamaEmbeddingModel(new OllamaProvider(url, options), id: modelName);
