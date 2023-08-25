namespace LangChain.Providers;

/// <inheritdoc cref="ModelIds.CodeLlama_34B" />
/// <param name="apiKey"></param>
/// <param name="httpClient"></param>
/// <exception cref="ArgumentNullException"></exception>
public class CodeLlamaLargeModel(string apiKey, HttpClient httpClient)
    : AnyscaleModel(apiKey, httpClient, id: ModelIds.CodeLlama_34B);