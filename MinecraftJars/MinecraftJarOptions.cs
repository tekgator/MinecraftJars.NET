namespace MinecraftJars;

public class MinecraftJarOptions
{
    /// <summary>
    /// If provided the CreateClient Method is utilized to create a HttpClient
    /// otherwise a new HttpClient is instantiated by the MinecraftJarManager
    /// </summary>     
    public IHttpClientFactory? HttpClientFactory { get; init; }    
}