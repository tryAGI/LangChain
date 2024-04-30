using System.Diagnostics.CodeAnalysis;
using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;
using LangChain.Serve.Classes.DTO;
using LangChain.Serve.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LangChain.Serve;

public static class ServeExtensions
{
    public static IServiceCollection AddLangChainServe(this IServiceCollection services)
    {
        services.AddSingleton<IConversationRepository, InMemoryRepository>();
        services.AddSingleton<IConversationNameProvider, DateConversationNameProvider>();
        return services;
    }

    public static IServiceCollection AddCustomNameGenerator(this IServiceCollection services, Func<IReadOnlyCollection<StoredMessage>, Task<string>> generator)
    {
        services.AddSingleton<IConversationNameProvider>(new CustomNameProvider(generator));
        return services;
    }

    [RequiresUnreferencedCode("This API may perform reflection on the supplied delegate and its parameters. These types may be trimmed if not directly referenced.")]
    [RequiresDynamicCode("This API may perform reflection on the supplied delegate and its parameters. These types may require generated code and aren't compatible with native AOT applications.")]
    [CLSCompliant(false)]
    public static WebApplication UseLangChainServe(this WebApplication app, Action<ServeOptions> options)
    {
        app = app ?? throw new ArgumentNullException(nameof(app));
        options = options ?? throw new ArgumentNullException(nameof(options));

        var serveMiddlewareOptions = new ServeOptions();
        options(serveMiddlewareOptions);
        var repository = app.Services.GetRequiredService<IConversationRepository>();
        var conversationNameProvider = app.Services.GetRequiredService<IConversationNameProvider>();
        var controller = new ServeController(serveMiddlewareOptions, repository, conversationNameProvider);

        app.MapGet("/serve/models", () => Results.Ok(controller.ListModels()));
        app.MapGet("/serve/conversations", async () => Results.Ok(await controller.ListConversations().ConfigureAwait(false)));
        app.MapPost("/serve/conversations", async (ConversationCreationDto conversationCreation) =>
        {
            var conversation = await controller.CreateConversation(conversationCreation.ModelName).ConfigureAwait(false);
            if (conversation == null)
            {
                return Results.NotFound("Model not found");
            }
            return Results.Ok(conversation);
        });
        app.MapPost("/serve/conversations/{conversationId:guid}/messages", async (PostMessageDto message, Guid conversationId) =>
        {
            var response = await controller.ProcessMessage(message, conversationId).ConfigureAwait(false);
            if (response == null)
            {
                return Results.NotFound("Conversation not found");
            }

            return Results.Ok(response);
        });
        app.MapGet("/serve/conversations/{conversationId:guid}", async (Guid conversationId) =>
        {
            var conversation = await controller.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                return Results.NotFound("Conversation not found");
            }

            return Results.Ok(conversation);
        });
        app.MapGet("/serve/conversations/{conversationId:guid}/messages", async (Guid conversationId) =>
        {
            var messages = await controller.ListMessages(conversationId).ConfigureAwait(false);
            if (messages == null)
            {
                return Results.NotFound("Conversation not found");
            }

            return Results.Ok(messages);
        });
        app.MapDelete("/serve/conversations/{conversationId:guid}", async (Guid conversationId) =>
        {
            await controller.DeleteConversation(conversationId).ConfigureAwait(false);

            return Results.Ok();
        });

        return app;
    }
}