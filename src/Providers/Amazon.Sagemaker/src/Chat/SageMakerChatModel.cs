using System.Diagnostics;
using System.Text;
using System.Text.Json;
using LangChain.Providers.Amazon.SageMaker.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.SageMaker;

public class SageMakerChatModel(
    SageMakerProvider provider,
    string sageMakerEndpointName)
    : ChatModel(id: sageMakerEndpointName ?? throw new ArgumentNullException(nameof(sageMakerEndpointName), "SageMaker Endpoint Name is not defined"))
{
    public override int ContextLength => 4096;

    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();
        var prompt = request.Messages.ToSimplePrompt();

        var usedSettings = SageMakerChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);
        
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                endpointName = Id,
                inputs = prompt,
                parameters = new
                {
                    max_new_tokens = usedSettings.MaxNewTokens
                }
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await provider.HttpClient.PostAsync(
            provider.Uri,
            jsonContent, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var generatedText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        var result = request.Messages.ToList();
        result.Add(generatedText.AsAiMessage());

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = result,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}