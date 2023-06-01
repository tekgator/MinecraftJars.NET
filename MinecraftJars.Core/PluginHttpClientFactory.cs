using System.Reflection;

namespace MinecraftJars.Core;

public class PluginHttpClientFactory
{
    private static readonly object LockObject = new();
    
    private readonly IHttpClientFactory? _httpClientFactory;
    private HttpClient? _httpClient;

    public PluginHttpClientFactory(IHttpClientFactory? httpClientFactory = null)
    {
        _httpClientFactory = httpClientFactory;
    }

    public HttpClient GetClient()
    {
        lock (LockObject)
        {
            HttpClient client;

            if (_httpClientFactory != null)
            {
                client = _httpClientFactory.CreateClient();            
            }
            else
            {
                _httpClient ??= new HttpClient(new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                });

                client = _httpClient;
            }

            if (!client.DefaultRequestHeaders.UserAgent.Any())
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);
            }
            
            return client;
        }
    }
}