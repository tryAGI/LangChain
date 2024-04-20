using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LangChain.Providers.Suno.Sdk;

public static class HttpClientExtensions
{
    public const string DefaultStagingApiKey ="#%VQBd*UO0z!T6r4l99GsWLmWS*Bzq%D@0wL#32J!RMf#iWkkABDu2&R*7Najf9F";
    public const string DefaultBaseAddress = "https://studio-api.suno.ai/api/";
    
    public static void Authorize(
        this HttpClient httpClient,
        string apiKey,
        string? stagingApiKey = null)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        stagingApiKey ??= DefaultStagingApiKey;

        httpClient.BaseAddress = new Uri(DefaultBaseAddress);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Add("Staging-Api-Key", stagingApiKey);
    }
    
    public static async Task<GenerateV2Response> GenerateV2Async(
        this HttpClient httpClient,
        GenerateV2Request request,
        CancellationToken cancellationToken = default)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        request = request ?? throw new ArgumentNullException(nameof(request));
#if NET6_0_OR_GREATER
        var createResponseMessage = await httpClient.PostAsJsonAsync(
            new Uri(httpClient.BaseAddress ?? new Uri(DefaultBaseAddress), "generate/v2/"),
            request,
            SourceGenerationContext.Default.GenerateV2Request,
            cancellationToken).ConfigureAwait(false);
        createResponseMessage.EnsureSuccessStatusCode();
        
        return await createResponseMessage.Content.ReadFromJsonAsync(
            SourceGenerationContext.Default.GenerateV2Response,
            cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Create response is null.");
#else
        var createResponseMessage = await httpClient.PostAsJsonAsync(
            new Uri(httpClient.BaseAddress ?? new Uri(DefaultBaseAddress), "generate/v2/"),
            request,
            cancellationToken).ConfigureAwait(false);
        createResponseMessage.EnsureSuccessStatusCode();
        
        return await createResponseMessage.Content.ReadFromJsonAsync<GenerateV2Response>(
            cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Create response is null.");
#endif
    }
    
    public static async Task<IReadOnlyList<Clip>> GetFeedAsync(
        this HttpClient httpClient,
        string[] ids,
        CancellationToken cancellationToken = default)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ids = ids ?? throw new ArgumentNullException(nameof(ids));
        
#if NET6_0_OR_GREATER
        return await httpClient.GetFromJsonAsync(
            new Uri(httpClient.BaseAddress ?? new Uri(DefaultBaseAddress), $"feed/?ids={string.Join("%2C", ids)}"),
            SourceGenerationContext.Default.ListClip,
            cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Clips is null.");
#else
        return await httpClient.GetFromJsonAsync<List<Clip>>(
            new Uri(httpClient.BaseAddress ?? new Uri(DefaultBaseAddress), $"feed/?ids={string.Join("%2C", ids)}"),
            cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Clips is null.");
#endif
    }
}