using LangChain.Providers.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LangChain.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddOpenAi(
        this IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        _ = services
            .AddOptions<OpenAiConfiguration>()
            .BindConfiguration(configSectionPath: OpenAiConfiguration.SectionName);
        _ = services
            .AddHttpClient<OpenAiProvider>();
        _ = services
            .AddScoped<OpenAiProvider>(static services => new OpenAiProvider(
                services.GetRequiredService<IOptions<OpenAiConfiguration>>().Value));

        return services;
    }
}