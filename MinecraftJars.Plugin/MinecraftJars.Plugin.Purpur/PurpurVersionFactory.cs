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
    
    public static async Task<List<PurpurVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var versions = new List<PurpurVersion>();
        var project = PurpurProjectFactory.Projects.Single(p => p.Name.Equals(projectName));
        
        using var client = GetHttpClient();

        var projectApi = await client.GetFromJsonAsync<Project>(PurpurProjectRequestUri, cancellationToken);
        
        if (projectApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
        
        if (!string.IsNullOrWhiteSpace(options.Version))
            projectApi.Versions.RemoveAll(v => !v.Equals(options.Version));
        
        projectApi.Versions.Reverse();
        versions.AddRange(projectApi.Versions
            .Select(projectApiVersion => new PurpurVersion(
                Project: project, 
                Version: projectApiVersion
            )));

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static async Task<IDownload> GetDownload(
        DownloadOptions options, 
        PurpurVersion version,
        CancellationToken cancellationToken)
    {
        using var client = GetHttpClient();
        
        var requestUriVersionBuilds = string.Format(PurpurVersionBuildsRequestUri, version.Version);
        var versionBuilds = await client.GetFromJsonAsync<VersionBuilds>(requestUriVersionBuilds, cancellationToken);

        if (versionBuilds == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var requestUriBuild = string.Format(PurpurBuildRequestUri, version.Version, versionBuilds.Builds.Latest);
        var build = await client.GetFromJsonAsync<Build>(requestUriBuild, cancellationToken);
        
        if (build == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var downloadUri = string.Format(PurpurDownloadRequestUri, version.Version, build.BuildId);

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{build.BuildId}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
                fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
            }
        }

        return new PurpurDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: build.BuildId,
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