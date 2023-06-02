using Microsoft.Extensions.DependencyInjection;
using MinecraftJars.Core.Providers;

namespace MinecraftJars.Extension.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinecraftJar(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IMinecraftJar, MinecraftJar>(sp => new MinecraftJar(new ProviderOptions
        {
            HttpClientFactory = sp.GetRequiredService<IHttpClientFactory>()
        }));

        return services;
    }
}