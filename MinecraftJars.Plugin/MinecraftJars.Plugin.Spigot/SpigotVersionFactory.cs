using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Spigot.Model;

namespace MinecraftJars.Plugin.Spigot;

internal static class SpigotVersionFactory
{
    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
    public static async Task<List<SpigotVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<SpigotVersion>();
        var projects = new List<SpigotProject>(SpigotProjectFactory.Projects);

        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));

        if (!projects.Any() || (options.Group is not null && options.Group is not (Group.Server or Group.Proxy)))
            return versions;

        using var client = GetHttpClient();

   
        return versions;
    }
    
    public static async Task<IDownload> GetDownload(SpigotVersion version)
    {
        using var client = GetHttpClient();
        
        
    }      
    
    private static HttpClient GetHttpClient()
    {
        var client = HttpClientFactory?.CreateClient() ?? new HttpClient();

        if (client.DefaultRequestHeaders.UserAgent.Any())
            return client;

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);

        return client;
    }
}