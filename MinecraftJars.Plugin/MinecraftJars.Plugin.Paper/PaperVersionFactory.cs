using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Paper.Model;
using MinecraftJars.Plugin.Paper.Model.BuildApi;
using MinecraftJars.Plugin.Paper.Model.ProjectApi;

namespace MinecraftJars.Plugin.Paper;

internal static class PaperVersionFactory
{
    public static IEnumerable<string> AvailableGameTypes => new List<string>
    {
        GameType.Paper,
        GameType.Folia,
        GameType.Waterfall,
        GameType.Velocity
    };
    
    private const string PaperProjectRequestUri = "https://api.papermc.io/v2/projects/{0}";
    private const string PaperBuildRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds";
    private const string PaperDownloadRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds/{2}/downloads/{3}";

    public static async Task<List<PaperVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<PaperVersion>();
        var gameTypes = new List<string>(AvailableGameTypes);

        if (options.GameType is not null)
            gameTypes.RemoveAll(t => !t.Equals(options.GameType));

        if (!gameTypes.Any() || (options.Group is not null && options.Group is not (Group.Server or Group.Proxy)))
            return versions;

        using var client = GetHttpClient();

        foreach (var gameType in gameTypes)
        {
            var project = await client
                              .GetFromJsonAsync<Project>(
                                  string.Format(PaperProjectRequestUri, gameType.ToLower()), 
                                  cancellationToken: cancellationToken);
            
            if (project == null) 
                throw new InvalidOperationException("Could not acquire game type details.");            
            
            var group = gameType is GameType.Paper or GameType.Folia ? Group.Server : Group.Proxy;
            
            versions.AddRange(project.Versions
                .Select(projectVersion => new PaperVersion
                {
                    Group = group, 
                    GameType = gameType, 
                    Version = projectVersion, 
                    Os = Os.Windows | Os.Linux | Os.MacOs
                }));
        }

        return versions;
    }
    
    public static async Task<IDownload> GetDownload(PaperVersion version)
    {
        using var client = GetHttpClient();
        
        var requestUri = string.Format(PaperBuildRequestUri, version.GameType.ToLower(), version.Version);
        var detail = await client.GetFromJsonAsync<BuildVersions>(requestUri);

        if (detail == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var build = detail.Builds.Last();
        version.ReleaseTime = build.Time;
            
        var downloadUri = string.Format(PaperDownloadRequestUri,
            version.GameType.ToLower(),version.Version,build.BuildId.ToString(),build.Downloads.Application.Name);
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        if (httpResponse.IsSuccessStatusCode)
            contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
            
        return new PaperDownload
        {
            FileName = build.Downloads.Application.Name,
            Size = contentLength,
            BuildId = build.BuildId,
            Url = downloadUri,
            HashType = HashType.Sha256,
            Hash = build.Downloads.Application.Sha256
        };
    }    

    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();

        if (client.DefaultRequestHeaders.UserAgent.Any())
            return client;

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);

        return client;
    }
}
