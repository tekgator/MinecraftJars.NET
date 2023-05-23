using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Purpur.Model;
using MinecraftJars.Plugin.Purpur.Model.BuildApi;
using MinecraftJars.Plugin.Purpur.Model.ProjectApi;

namespace MinecraftJars.Plugin.Purpur;

internal static class PurpurVersionFactory
{
    private const string PurpurProjectRequestUri = "https://api.purpurmc.org/v2/purpur/";
    private const string PurpurVersionBuildsRequestUri = "https://api.purpurmc.org/v2/purpur/{0}";
    private const string PurpurBuildRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}";
    private const string PurpurDownloadRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}/download";
    
    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
    public static async Task<List<PurpurVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<PurpurVersion>();
        var projects = new List<PurpurProject>(PurpurProjectFactory.Projects);

        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));

        if (!projects.Any() || (options.Group is not null && options.Group is not Group.Server))
            return versions;

        using var client = GetHttpClient();

        var projectApi = await client.GetFromJsonAsync<Project>(PurpurProjectRequestUri, cancellationToken);
        
        if (projectApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
        
        foreach (var project in projects.Where(p => p.Name.Equals(projectApi.ProjectName, StringComparison.OrdinalIgnoreCase)))
        {
            if (options.Version is not null)
                projectApi.Versions.RemoveAll(v => !v.Equals(options.Version));
            
            projectApi.Versions.Reverse();
            versions.AddRange(projectApi.Versions
                .Select(projectApiVersion => new PurpurVersion(
                    Project: project, 
                    Version: projectApiVersion
                )));
        }

        return versions;
    }
    
    public static async Task<IDownload> GetDownload(DownloadOptions options, PurpurVersion version)
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

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{build.BuildId}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (httpResponse.IsSuccessStatusCode)
            {
                contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
                fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
            }
        }

        return new PurpurDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: int.Parse(build.BuildId),
            Url: downloadUri,
            ReleaseTime: DateTimeOffset.FromUnixTimeMilliseconds(build.Timestamp).DateTime,
            HashType: HashType.Md5,
            Hash: build.Md5);
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