using System.Reflection;
using Tgs.Payment.WFPay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWFPay(
        this IServiceCollection services,
        Action<WFPayOptions, IServiceProvider> configureOptions,
        Action<IHttpClientBuilder> ConfigureAiClient)
    {
        ConfigureAiClient?.Invoke(services.AddHttpClient<WFPayClient>());

        return services
            .AddOptions<WFPayOptions>()
            .Configure(configureOptions)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddSingleton<WFPaySigner>();
    }
}