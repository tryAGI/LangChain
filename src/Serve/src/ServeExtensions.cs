using LangChain.Serve.Classes.DTO;
using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;
using LangChain.Utilities.Services;
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

    public static IServiceCollection AddCustomNameGenerator(this IServiceCollection services,Func<List<StoredMessage>,Task<string>> generator)
    {
        services.AddSingleton<IConversationNameProvider>(new CustomNameProvider(generator));
        return services;
    }

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
        app.MapGet("/serve/conversations/list", async () => Results.Ok(await controller.ListConversations().ConfigureAwait(false)));
        app.MapPost("/serve/conversations/new", async (ConversationCreationDTO conversationCreation) =>
        {
            var conversation = await controller.CreateConversation(conversationCreation.ModelName).ConfigureAwait(false);
            if (conversation == null)
            {
                throw new InvalidOperationException("Model not found");
            }
            return conversation;
        });
        app.MapPost("/serve/conversations/{conversationId}/messages", async ( PostMessageDTO message, Guid conversationId) =>
        {
            var response = await controller.ProcessMessage(message, conversationId).ConfigureAwait(false);
            if (response == null)
            {
                throw new InvalidOperationException("Conversation not found");
            }
            return response;
        });
        app.MapGet("/serve/conversations/{conversationId}", async (Guid conversationId) =>
        {
            var conversation = await controller.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                throw new InvalidOperationException("Conversation not found");
            }
            return conversation;
        });
        app.MapGet("/serve/conversations/{conversationId}/messages", async (Guid conversationId) =>
        {
            var messages = await controller.ListMessages(conversationId).ConfigureAwait(false);
            if (messages == null)
            {
                throw new InvalidOperationException("Conversation not found");
            }
            return messages;
        });
        app.MapDelete("/serve/conversations/{conversationId}", async (Guid conversationId) =>
        {
            await controller.DeleteConversation(conversationId).ConfigureAwait(false);
            
            return Results.Ok();
        });


        return app;

    }
}