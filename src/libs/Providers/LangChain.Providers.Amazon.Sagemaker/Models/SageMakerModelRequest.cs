using System.Text;
using System.Text.Json;

namespace LangChain.Providers.Amazon.SageMaker.Models;

public class SageMakerModelRequest : ISagemakerModelRequest
{
    public async Task<string> GenerateAsync(HttpClient httpClient, ChatRequest request, SageMakerConfiguration configuration)
    {
        var prompt = ToPrompt(request.Messages);

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                endpointName = configuration.ModelId,
                inputs = prompt,
                parameters = new
                {
                    max_new_tokens = configuration.MaxNewTokens
                }
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync(
            configuration.Url,
            jsonContent);

        response.EnsureSuccessStatusCode();

        var generatedText = await response.Content.ReadAsStringAsync();

        return generatedText;
    }

    public string ToPrompt(IReadOnlyCollection<Message> messages)
    {
        var sb = new StringBuilder();

        foreach (var item in messages)
        {
            sb.Append(item.Content);
        }
        return sb.ToString();
    }
}