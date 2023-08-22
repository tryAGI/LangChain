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
    public static IServiceCollection AddHuggingFace(
        this IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        _ = services
            .AddOptions<HuggingFaceConfiguration>()
            .BindConfiguration(configSectionPath: HuggingFaceConfiguration.SectionName);
        _ = services
            .AddHttpClient<HuggingFaceModel>();
        _ = services
            .AddScoped<HuggingFaceModel>(static services => new HuggingFaceModel(
                configuration: services.GetRequiredService<IOptions<HuggingFaceConfiguration>>().Value,
                httpClient: services.GetRequiredService<HttpClient>()));

        return services;
    }
}