using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

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
        var controller = new ServeController(serveMiddlewareOptions);

        app.MapGet("/v1/models", () => Results.Ok(controller.ListModels()));
        app.MapPost("/v1/chat/completions", async (ChatRequest request) =>
        {
            var llm = serveMiddlewareOptions.GetModel(request.Model);

            var response = await llm.GenerateAsync(new LangChain.Providers.ChatRequest
            {
                Messages = request.Messages.Select(x => new LangChain.Providers.Message
                {
                    Content = x.Content,
                    Role = x.Role switch
                    {
                        Role.Assistant => Providers.MessageRole.Ai,
                        Role.System => Providers.MessageRole.System,
                        Role.User => Providers.MessageRole.Human,
                        _ => throw new NotImplementedException(),
                    }
                }).ToList(),
            }).ConfigureAwait(false);

            return Results.Ok(new ChatResponse());
        });

        return app;
    }
}