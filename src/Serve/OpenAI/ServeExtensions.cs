using System.Diagnostics.CodeAnalysis;
using LangChain.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

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
        // var repository = app.Services.GetRequiredService<IConversationRepository>();
        // var conversationNameProvider = app.Services.GetRequiredService<IConversationNameProvider>();
        //var controller = new ServeController(serveMiddlewareOptions);

        //app.MapGet("/v1/models", () => Results.Ok(controller.ListModels()));
        app.MapPost("/v1/chat/completions", async (CreateChatCompletionRequest request) =>
        {
            var llm = serveMiddlewareOptions.GetModel(request.Model.Value1 ?? request.Model.Value2?.ToValueString() ?? string.Empty);

            var response = await llm.GenerateAsync(new ChatRequest
            {
                Messages = request.Messages.Select(x => new Message
                {
                    Content = x.User?.Content.Value1 ?? x.Assistant?.Content?.Value1 ?? x.System?.Content.Value1 ?? string.Empty,
                    Role = x.Object switch
                    {
                        ChatCompletionRequestAssistantMessage => MessageRole.Ai,
                        ChatCompletionRequestSystemMessage => MessageRole.System,
                        ChatCompletionRequestUserMessage => MessageRole.Human,
                        _ => throw new NotImplementedException(),
                    }
                }).ToList(),
            });

            return Results.Ok(new CreateChatCompletionResponse
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTimeOffset.UtcNow,
                Model = request.Model.Value1 ?? request.Model.Value2?.ToValueString() ?? string.Empty,
                Object = CreateChatCompletionResponseObject.ChatCompletion,
                Choices =
                [
                    new CreateChatCompletionResponseChoice
                    {
                        Message = new ChatCompletionResponseMessage
                        {
                            Content = response.LastMessageContent,
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