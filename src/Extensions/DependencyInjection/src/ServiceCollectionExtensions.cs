using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace LangChain.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering MEAI services used by LangChain chains.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IChatClient"/> singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="chatClient">The chat client instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddChatClient(
        this IServiceCollection services,
        IChatClient chatClient)
    {
        services.AddSingleton(chatClient);
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IChatClient"/> singleton using a factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory to create the chat client.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddChatClient(
        this IServiceCollection services,
        Func<IServiceProvider, IChatClient> factory)
    {
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IEmbeddingGenerator{String, Embedding}"/> singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="embeddingGenerator">The embedding generator instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmbeddingGenerator(
        this IServiceCollection services,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        services.AddSingleton(embeddingGenerator);
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IEmbeddingGenerator{String, Embedding}"/> singleton using a factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory to create the embedding generator.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmbeddingGenerator(
        this IServiceCollection services,
        Func<IServiceProvider, IEmbeddingGenerator<string, Embedding<float>>> factory)
    {
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Registers an <see cref="ISpeechToTextClient"/> singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="speechToTextClient">The speech-to-text client instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSpeechToTextClient(
        this IServiceCollection services,
        ISpeechToTextClient speechToTextClient)
    {
        services.AddSingleton(speechToTextClient);
        return services;
    }

    /// <summary>
    /// Registers an <see cref="ISpeechToTextClient"/> singleton using a factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory to create the speech-to-text client.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSpeechToTextClient(
        this IServiceCollection services,
        Func<IServiceProvider, ISpeechToTextClient> factory)
    {
        services.AddSingleton(factory);
        return services;
    }
}
