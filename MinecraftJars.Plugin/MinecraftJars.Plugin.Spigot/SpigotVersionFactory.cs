using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Spigot.Model;
using MinecraftJars.Plugin.Spigot.Model.JenkinsApi;

namespace MinecraftJars.Plugin.Spigot;

internal static class SpigotVersionFactory
{
    private const string BungeeCoordRequestUri = "https://ci.md-5.net/job/BungeeCord/api/json?tree=builds[number,url,result,inProgress,timestamp,artifacts[fileName,relativePath]]";
    private const string BungeeCoordRequestUriMaxRecordSuffix = "{{0,{0}}}";
    
    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
    public static async Task<List<SpigotVersion>> Get(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var taskSpigot = Task.FromResult(new List<SpigotVersion>());
        var taskBungeeCoord = GetBungeeCoord(options, cancellationToken);

        await Task.WhenAll(taskSpigot, taskBungeeCoord);

        return (await taskSpigot).Concat(await taskBungeeCoord).ToList();
    }
    
    private static async Task<List<SpigotVersion>> GetBungeeCoord(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<SpigotVersion>();
        var projects = new List<SpigotProject>(SpigotProjectFactory.Projects);

        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));

        if (!projects.Any() || (options.Group is not null && options.Group is not Group.Proxy))
            return versions;

        var project = projects.FirstOrDefault(p => p.Name.Equals(SpigotProjectFactory.BungeeCord));
        if (project == null)
            return versions;
        
        using var client = GetHttpClient();

        var requestUrl = BungeeCoordRequestUri + (options.MaxRecords == null
            ? string.Empty
            : string.Format(BungeeCoordRequestUriMaxRecordSuffix, options.MaxRecords));
        
        var job = await client.GetFromJsonAsync<Job>(requestUrl, cancellationToken);
            
        if (job == null)
            throw new InvalidOperationException("Could not acquire version details.");

        versions.AddRange(job.Builds
            .Where(b => !b.InProgress && 
                        b.Result.Equals("success", StringComparison.OrdinalIgnoreCase))
            .Select(b => new SpigotVersion(
                project, 
                b.Number.ToString())
                {
                    ReleaseTime = DateTimeOffset.FromUnixTimeMilliseconds(b.Timestamp).DateTime, 
                    DetailUrl = $"{b.Url}artifact/{b.Artifacts.First().RelativePath}"
                }));

        return versions;
    }
    
    public static async Task<IDownload> GetDownload(DownloadOptions options, SpigotVersion version)
    {
        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}.jar";
        
        if (options.LoadFilesize)
        {
            using var client = GetHttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? 0;
        }

        return new SpigotDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: int.Parse(version.Version),
            Url: version.DetailUrl!,
            ReleaseTime: version.ReleaseTime);
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