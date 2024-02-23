using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class MetaLlama2ChatModel(
    BedrockProvider provider,
    string id)
    : ChatModel(id)
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

        // TODO: implement settings
        // var usedSettings = MetaLlama2ChatSettings.Calculate(
        //     requestSettings: settings,
        //     modelSettings: Settings,
        //     providerSettings: provider.ChatSettings);
        var response = await provider.Api.InvokeModelAsync(
            Id,
            new JsonObject
            {
                { "prompt", prompt },
                { "max_gen_len", 512 },
                { "temperature", 0.5 },
                { "top_p", 0.9 },
            },
            cancellationToken).ConfigureAwait(false);

        var generatedText = response?["generation"]?
            .GetValue<string>() ?? string.Empty;

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
            UsedSettings = ChatSettings.Default,
            Usage = usage,
        };
    }
}