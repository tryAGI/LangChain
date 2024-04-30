using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class OllamaApiClient
{
    private readonly HttpClient _client;
    private JsonSerializerOptions? _jsonSerializerOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    public OllamaApiClient(string url)
        : this(new HttpClient { BaseAddress = new Uri(url) })
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OllamaApiClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Model>> ListLocalModels()
    {
        var data = await GetAsync<ListModelsResponse>("/api/tags")
            .ConfigureAwait(false);
        return data.Models;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public async Task PullModel(string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/pull");
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            name,
            stream = false,
        }), Encoding.UTF8, "application/json");
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    private async Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<TResponse>(responseBody) ?? throw new InvalidOperationException("Response body was null");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateRequest"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<GenerateCompletionResponseStream> GenerateCompletion(
        GenerateCompletionRequest generateRequest)
    {
        generateRequest = generateRequest ?? throw new ArgumentNullException(nameof(generateRequest));

        _jsonSerializerOptions ??= new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var content = JsonSerializer.Serialize(generateRequest, _jsonSerializerOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate");
        request.Content = new StringContent(content, Encoding.UTF8, "application/json");

        var completion = generateRequest.Stream
            ? HttpCompletionOption.ResponseHeadersRead
            : HttpCompletionOption.ResponseContentRead;

        using var response = await _client.SendAsync(request, completion).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
            var streamedResponse = JsonSerializer.Deserialize<GenerateCompletionResponseStream>(line) ??
                                   throw new InvalidOperationException("Response body was null");

            yield return streamedResponse;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateRequest"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<GenerateEmbeddingResponse> GenerateEmbeddings(GenerateEmbeddingRequest generateRequest)
    {
        _jsonSerializerOptions ??= new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var content = JsonSerializer.Serialize(generateRequest, _jsonSerializerOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/embeddings");
        request.Content = new StringContent(content, Encoding.UTF8, "application/json");

        var completion = HttpCompletionOption.ResponseContentRead;

        using var response = await _client.SendAsync(request, completion).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        string line = await reader.ReadToEndAsync().ConfigureAwait(false);
        var streamedResponse = JsonSerializer.Deserialize<GenerateEmbeddingResponse>(line) ?? throw new InvalidOperationException("Response body was null");
        return streamedResponse;
    }
}
