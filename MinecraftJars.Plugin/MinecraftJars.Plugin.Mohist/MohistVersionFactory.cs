using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mohist.Model;
using MinecraftJars.Plugin.Mohist.Model.BuildApi;

namespace MinecraftJars.Plugin.Mohist;

internal static class MohistVersionFactory
{
    public static IEnumerable<string> AvailableGameTypes => new List<string>
    {
        GameType.Mohist
    };
    
    private const string MohistVersionRequestUri = "https://mohistmc.com/api/versions";
    private const string MohistLatestBuildRequestUri = "https://mohistmc.com/api/{0}/latest/";
    
    public static async Task<List<MohistVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<MohistVersion>();
        var gameTypes = new List<string>(AvailableGameTypes);

        if (options.GameType is not null)
            gameTypes.RemoveAll(t => !t.Equals(options.GameType));

        if (!gameTypes.Any() || (options.Group is not null && options.Group is not Group.Server))
            return versions;
        
        using var client = GetHttpClient();
        var availVersions = await client.GetFromJsonAsync<List<string>>(MohistVersionRequestUri, cancellationToken);        
        
        if (availVersions == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
       
        availVersions.Reverse();

        versions.AddRange(availVersions
            .Select(version => new MohistVersion
            {
                Group = Group.Server, 
                GameType = GameType.Mohist, 
                Version = version, 
                Os = Os.Windows | Os.Linux | Os.MacOs
            }));
        return versions;
    }

    public static async Task<IDownload> GetDownload(MohistVersion version)
    {
        using var client = GetHttpClient();
        
        var requestUriLatestBuild = string.Format(MohistLatestBuildRequestUri, version.Version);
        var latestBuild = await client.GetFromJsonAsync<Build>(requestUriLatestBuild);

        if (latestBuild == null || string.IsNullOrWhiteSpace(latestBuild.Url)) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, latestBuild.Url);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        var fileName = $"{version.GameType.ToLower()}-{version.Version}-{latestBuild.Number}.jar";
        if (httpResponse.IsSuccessStatusCode)
        {
            contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
            fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
        }
            
        return new MohistDownload
        {
            FileName = fileName,
            Size = contentLength,
            BuildId = latestBuild.Number,
            Url = latestBuild.Url,
            ReleaseTime = DateTimeOffset.FromUnixTimeMilliseconds(latestBuild.Timeinmillis).DateTime,
            HashType = HashType.Md5,
            Hash = latestBuild.Md5
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