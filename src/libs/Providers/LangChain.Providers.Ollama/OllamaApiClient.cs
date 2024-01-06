using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace OllamaTest;

public class OllamaApiClient
{

    private readonly HttpClient _client;

    public OllamaApiClient(string url)
        : this(new HttpClient() { BaseAddress = new Uri(url) })
    {
    }

    public OllamaApiClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IEnumerable<Model>> ListLocalModels()
    {
        var data = await GetAsync<ListModelsResponse>("/api/tags");
        return data.Models;
    }

    public async Task PullModel(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/pull")
        {
            Content = new StringContent(JsonSerializer.Serialize(new { name, stream=false }), Encoding.UTF8, "application/json")
        };
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        await reader.ReadToEndAsync();
    }



    private async Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseBody);
    }


    public async IAsyncEnumerable<GenerateCompletionResponseStream> GenerateCompletion(GenerateCompletionRequest generateRequest)
    {
        var content = JsonSerializer.Serialize(generateRequest, new JsonSerializerOptions(){DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull});
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        var completion = generateRequest.Stream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead;

        using var response = await _client.SendAsync(request, completion);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            var streamedResponse = JsonSerializer.Deserialize<GenerateCompletionResponseStream>(line);
            
            yield return streamedResponse;
        }
    }

    public async Task<GenerateEmbeddingResponse> GenerateEmbeddings(GenerateEmbeddingRequest generateRequest)
    {
        var content = JsonSerializer.Serialize(generateRequest, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        var completion = HttpCompletionOption.ResponseContentRead;

        using var response = await _client.SendAsync(request, completion);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        string line = await reader.ReadToEndAsync();
        var streamedResponse = JsonSerializer.Deserialize<GenerateEmbeddingResponse>(line);
        return streamedResponse;

    }


}