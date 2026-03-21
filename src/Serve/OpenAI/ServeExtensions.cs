using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using tryAGI.OpenAI;

namespace LangChain.Serve.OpenAI;

public static class ServeExtensions
{
    public static IServiceCollection AddLangChainServeOpenAi(this IServiceCollection services)
    {
        return services;
    }

    [RequiresUnreferencedCode("This API may perform reflection on the supplied delegate and its parameters. These types may be trimmed if not directly referenced.")]
    [RequiresDynamicCode("This API may perform reflection on the supplied delegate and its parameters. These types may require generated code and aren't compatible with native AOT applications.")]
    [CLSCompliant(false)]
    public static WebApplication UseLangChainServeOpenAi(this WebApplication app, Action<ServeOptions> options)
    {
        app = app ?? throw new ArgumentNullException(nameof(app));
        options = options ?? throw new ArgumentNullException(nameof(options));

        var serveMiddlewareOptions = new ServeOptions();
        options(serveMiddlewareOptions);

        app.MapPost("/v1/chat/completions", async (CreateChatCompletionRequest request) =>
        {
            var modelId = request.Model.Value1 ?? request.Model.Value2?.ToValueString() ?? string.Empty;

            var chatClient = serveMiddlewareOptions.GetChatClient(modelId)
                ?? throw new InvalidOperationException($"Model '{modelId}' is not registered.");

            var messages = request.Messages.Select(x => new ChatMessage(
                x.Object switch
                {
                    ChatCompletionRequestAssistantMessage => ChatRole.Assistant,
                    ChatCompletionRequestSystemMessage => ChatRole.System,
                    ChatCompletionRequestUserMessage => ChatRole.User,
                    _ => ChatRole.User,
                },
                x.User?.Content.Value1 ?? x.Assistant?.Content?.Value1 ?? x.System?.Content.Value1 ?? string.Empty
            )).ToList();

            var chatResponse = await chatClient.GetResponseAsync(messages);
            var responseContent = chatResponse.Text ?? string.Empty;

            return Results.Ok(new CreateChatCompletionResponse
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTimeOffset.UtcNow,
                Model = modelId,
                Object = CreateChatCompletionResponseObject.ChatCompletion,
                Choices =
                [
                    new CreateChatCompletionResponseChoice
                    {
                        Message = new ChatCompletionResponseMessage
                        {
                            Content = responseContent,
                            Role = ChatCompletionResponseMessageRole.Assistant,
                        },
                        Index = 0,
                        Logprobs = new CreateChatCompletionResponseChoiceLogprobs
                        {
                            Content = [],
                            Refusal = [],
                        },
                        FinishReason = CreateChatCompletionResponseChoiceFinishReason.Stop,
                    }
                ],
            });
        });

        return app;
    }
}
