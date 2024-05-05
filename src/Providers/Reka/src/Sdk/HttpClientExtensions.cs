using System.Net.Http.Headers;
using System.Net.Http.Json;

#pragma warning disable IL2026
#pragma warning disable IL3050

namespace Reka;

public static class HttpClientExtensions
{
    public const string DefaultBaseAddress = "https://api.reka.ai/";

    public static void Authorize(
        this HttpClient httpClient,
        string apiKey,
        Uri? baseAddress = null)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

        httpClient.BaseAddress = baseAddress ?? new Uri(DefaultBaseAddress);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<ChatRoundResponse> ChatAsync(
        this HttpClient httpClient,
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var createResponseMessage = await httpClient.PostAsJsonAsync(
            new Uri(httpClient.BaseAddress ?? new Uri(DefaultBaseAddress), "chat"),
            request,
            cancellationToken).ConfigureAwait(false);
        try
        {
            createResponseMessage.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var validationError = await createResponseMessage.Content.ReadFromJsonAsync<HTTPValidationError>(cancellationToken: cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException(
                $"Failed to chat with Reka API. Status code: {createResponseMessage.StatusCode}. Reason: {ex.Message}. Validation error: {validationError?.Detail?.ElementAtOrDefault(0)?.Msg}.",
                ex);
        }

        return await createResponseMessage.Content.ReadFromJsonAsync<ChatRoundResponse>(
            cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Create response is null.");
    }
}