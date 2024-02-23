using LangChain.Chains.HelperChains;
using LangChain.Serve.Classes.DTO;
using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;
using LangChain.Utilities.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

    public static WebApplication UseLangChainServe(this WebApplication app, Action<ServeOptions> options)
    {
        var serveMiddlewareOptions = new ServeOptions();
        options(serveMiddlewareOptions);
        var repository = (IConversationRepository)app.Services.GetService(typeof(IConversationRepository));
        var conversationNameProvider = (IConversationNameProvider)app.Services.GetService(typeof(IConversationNameProvider));
        var controller = new ServeController(serveMiddlewareOptions, repository, conversationNameProvider);

        app.MapGet("/serve/models", () => Results.Ok(controller.ListModels()));
        app.MapGet("/serve/conversations/list", async () => Results.Ok(await controller.ListConversations()));
        app.MapPost("/serve/conversations/new", async (ConversationCreationDTO conversationCreation) =>
        {
            var conversation = await controller.CreateConversation(conversationCreation.ModelName);
            if (conversation == null)
            {
                throw new Exception("Model not found");
            }
            return conversation;
        });
        app.MapPost("/serve/conversations/{conversationId}/messages", async ( PostMessageDTO message, Guid conversationId) =>
        {
            var response = await controller.ProcessMessage(message, conversationId);
            if (response == null)
            {
                throw new Exception("Conversation not found");
            }
            return response;
        });
        app.MapGet("/serve/conversations/{conversationId}", async (Guid conversationId) =>
        {
            var conversation = await controller.GetConversation(conversationId);
            if (conversation == null)
            {
                throw new Exception("Conversation not found");
            }
            return conversation;
        });
        app.MapGet("/serve/conversations/{conversationId}/messages", async (Guid conversationId) =>
        {
            var messages = await controller.ListMessages(conversationId);
            if (messages == null)
            {
                throw new Exception("Conversation not found");
            }
            return messages;
        });
        app.MapDelete("/serve/conversations/{conversationId}", async (Guid conversationId) =>
        {
            await controller.DeleteConversation(conversationId);
            
            return Results.Ok();
        });


        return app;

    }
}