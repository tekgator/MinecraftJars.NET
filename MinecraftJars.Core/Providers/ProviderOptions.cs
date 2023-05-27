namespace MinecraftJars.Core.Providers;

public class ProviderOptions
{
    /// <summary>
    /// If provided the CreatClient Method is utilized to create a HttpClient
    /// otherwise a new HttpClient is instantiated by each plugin
    /// </summary>     
    public IHttpClientFactory? HttpClientFactory { get; init; }
}