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
    public static IServiceCollection AddAnthropic(
        this IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        _ = services
            .AddOptions<AnthropicConfiguration>()
            .BindConfiguration(configSectionPath: "Anthropic");
        _ = services
            .AddHttpClient<AnthropicModel>();
        _ = services
            .AddScoped<AnthropicModel>(static services => new AnthropicModel(
                configuration: services.GetRequiredService<IOptions<AnthropicConfiguration>>().Value,
                httpClient: services.GetRequiredService<HttpClient>()));
        
        return services;
    }
}