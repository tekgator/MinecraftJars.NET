using System.Reflection;

namespace MinecraftJars.Core.Providers;

public class ProviderOptions
{
    /// <summary>
    /// If provided the CreatClient Method is utilized to create a HttpClient
    /// otherwise a new HttpClient is instantiated by each plugin
    /// </summary>     
    public IHttpClientFactory? HttpClientFactory { get; init; }
    
    public HttpClient GetHttpClient()
    {
        var client = HttpClientFactory?.CreateClient() ?? new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        });

        if (client.DefaultRequestHeaders.UserAgent.Any())
            return client;

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);

        return client;
    }    
}