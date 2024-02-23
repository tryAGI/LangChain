using LangChain.Providers.Anyscale;
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
    public static IServiceCollection AddAnyscale(
        this IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        _ = services
            .AddOptions<AnyscaleConfiguration>()
            .BindConfiguration(configSectionPath: AnyscaleConfiguration.SectionName);
        _ = services
            .AddHttpClient<AnyscaleProvider>();
        _ = services
            .AddScoped<AnyscaleProvider>(static services => new AnyscaleProvider(
                configuration: services.GetRequiredService<IOptions<AnyscaleConfiguration>>().Value));

        return services;
    }
}