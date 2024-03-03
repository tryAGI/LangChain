using System.Diagnostics;
using System.Text;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.SageMaker;

public class SageMakerModel(
    SageMakerProvider provider,
    string endpointName)
    : ChatModel(id: endpointName ?? throw new ArgumentNullException(nameof(endpointName), "SageMaker Endpoint Name is not defined"))
{
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var messages = request.Messages.ToList();

        var watch = Stopwatch.StartNew();

        var usedSettings = SageMakerSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);
        usedSettings.InputParamers?.Add("endpointName", Id);

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(usedSettings.InputParamers),
            Encoding.UTF8,
            usedSettings.ContentType!);

        using var response = await provider.HttpClient.PostAsync(provider.Uri, jsonContent, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var output = usedSettings.TransformOutput!(response);
        messages.Add(new Message(output!, MessageRole.Ai));

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}