using System.Diagnostics.CodeAnalysis;
using LangChain.Providers.HuggingFace;
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
    [RequiresDynamicCode("Requires dynamic code.")]
    [RequiresUnreferencedCode("Requires unreferenced code.")]
    public static IServiceCollection AddHuggingFace(
        this IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        _ = services
            .AddOptions<HuggingFaceConfiguration>()
            .BindConfiguration(configSectionPath: HuggingFaceConfiguration.SectionName);
        _ = services
            .AddHttpClient<HuggingFaceProvider>();
        _ = services
            .AddScoped<HuggingFaceProvider>(static services => new HuggingFaceProvider(
                configuration: services.GetRequiredService<IOptions<HuggingFaceConfiguration>>().Value,
                httpClient: services.GetRequiredService<HttpClient>()));

        return services;
    }
}