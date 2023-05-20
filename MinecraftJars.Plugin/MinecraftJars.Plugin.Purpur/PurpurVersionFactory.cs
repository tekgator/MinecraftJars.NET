using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Purpur.Model;
using MinecraftJars.Plugin.Purpur.Model.BuildApi;
using MinecraftJars.Plugin.Purpur.Model.ProjectApi;

namespace MinecraftJars.Plugin.Purpur;

internal static class PurpurVersionFactory
{
    public static IEnumerable<string> AvailableGameTypes => new List<string>
    {
        GameType.Purpur
    };
    
    private const string PurpurProjectRequestUri = "https://api.purpurmc.org/v2/purpur/";
    private const string PurpurVersionBuildsRequestUri = "https://api.purpurmc.org/v2/purpur/{0}";
    private const string PurpurBuildRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}";
    private const string PurpurDownloadRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}/download";
    
    public static async Task<List<PurpurVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<PurpurVersion>();
        var gameTypes = new List<string>(AvailableGameTypes);

        if (options.GameType is not null)
            gameTypes.RemoveAll(t => !t.Equals(options.GameType));

        if (!gameTypes.Any() || (options.Group is not null && options.Group is not Group.Server))
            return versions;

        using var client = GetHttpClient();

        var project = await client.GetFromJsonAsync<Project>(PurpurProjectRequestUri, cancellationToken);
        
        if (project == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
        
        project.Versions.Reverse();
        
        versions.AddRange(project.Versions
            .Select(projectVersion => new PurpurVersion
            {
                Group = Group.Server, 
                GameType = GameType.Purpur, 
                Version = projectVersion
            }));

        return versions;
    }
    
    public static async Task<IDownload> GetDownload(PurpurVersion version)
    {
        using var client = GetHttpClient();
        
        var requestUriVersionBuilds = string.Format(PurpurVersionBuildsRequestUri, version.Version);
        var versionBuilds = await client.GetFromJsonAsync<VersionBuilds>(requestUriVersionBuilds);

        if (versionBuilds == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var requestUriBuild = string.Format(PurpurBuildRequestUri, version.Version, versionBuilds.Builds.Latest);
        var build = await client.GetFromJsonAsync<Build>(requestUriBuild);
        
        if (build == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var downloadUri = string.Format(PurpurDownloadRequestUri, version.Version, build.BuildId);
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        var fileName = $"{version.GameType.ToLower()}-{version.Version}-{build.BuildId}.jar";
        if (httpResponse.IsSuccessStatusCode)
        {
            contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
            fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
        }
            
        return new PurpurDownload
        {
            FileName = fileName,
            Size = contentLength,
            BuildId = int.Parse(build.BuildId),
            Url = downloadUri,
            ReleaseTime = DateTimeOffset.FromUnixTimeMilliseconds(build.Timestamp).DateTime,
            HashType = HashType.Md5,
            Hash = build.Md5
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