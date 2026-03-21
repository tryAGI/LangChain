using System.Diagnostics.CodeAnalysis;
using LangChain.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using tryAGI.OpenAI;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using AiChatRole = Microsoft.Extensions.AI.ChatRole;

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
            string responseContent;

            var chatClient = serveMiddlewareOptions.GetChatClient(modelId);
            if (chatClient != null)
            {
                // MEAI IChatClient path
                var messages = request.Messages.Select(x => new AiChatMessage(
                    x.Object switch
                    {
                        ChatCompletionRequestAssistantMessage => AiChatRole.Assistant,
                        ChatCompletionRequestSystemMessage => AiChatRole.System,
                        ChatCompletionRequestUserMessage => AiChatRole.User,
                        _ => AiChatRole.User,
                    },
                    x.User?.Content.Value1 ?? x.Assistant?.Content?.Value1 ?? x.System?.Content.Value1 ?? string.Empty
                )).ToList();

                var chatResponse = await chatClient.GetResponseAsync(messages);
                responseContent = chatResponse.Text ?? string.Empty;
            }
            else
            {
                // Legacy ChatModel path
                var llm = serveMiddlewareOptions.GetModel(modelId);

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

                responseContent = response.LastMessageContent;
            }

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