using LangChain.Providers;
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
            .AddHttpClient<OpenAiModel>();
        _ = services
            .AddScoped<OpenAiModel>(static services => new OpenAiModel(
                configuration: services.GetRequiredService<IOptions<OpenAiConfiguration>>().Value,
                httpClient: services.GetRequiredService<HttpClient>()));
        
        return services;
    }
}