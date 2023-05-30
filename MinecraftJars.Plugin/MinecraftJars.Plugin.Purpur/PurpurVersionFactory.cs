using System.Net.Http.Json;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Purpur.Model;
using MinecraftJars.Plugin.Purpur.Model.PurpurApi;

namespace MinecraftJars.Plugin.Purpur;

internal static class PurpurVersionFactory
{
    private const string PurpurProjectRequestUri = "https://api.purpurmc.org/v2/purpur/";
    private const string PurpurVersionBuildsRequestUri = "https://api.purpurmc.org/v2/purpur/{0}";
    private const string PurpurBuildRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}";
    private const string PurpurDownloadRequestUri = "https://api.purpurmc.org/v2/purpur/{0}/{1}/download";
    
    public static HttpClient HttpClient { get; set; } = default!;
    
    public static async Task<List<PurpurVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = PurpurProjectFactory.Projects.Single(p => p.Name.Equals(projectName));
        
        var projectApi = await HttpClient.GetFromJsonAsync<Project>(PurpurProjectRequestUri, cancellationToken);
        
        if (projectApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
        
        if (!string.IsNullOrWhiteSpace(options.Version))
            projectApi.Versions.RemoveAll(v => !v.Equals(options.Version));
        
        projectApi.Versions.Reverse();

        var versions = projectApi.Versions
            .Select(projectApiVersion => new PurpurVersion(
                Project: project, 
                Version: projectApiVersion)
            ).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static async Task<IDownload> GetDownload(
        DownloadOptions options, 
        PurpurVersion version,
        CancellationToken cancellationToken)
    {
        var requestUriVersionBuilds = string.Format(PurpurVersionBuildsRequestUri, version.Version);
        var versionBuilds = await HttpClient.GetFromJsonAsync<VersionBuilds>(requestUriVersionBuilds, cancellationToken);

        if (versionBuilds == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var requestUriBuild = string.Format(PurpurBuildRequestUri, version.Version, versionBuilds.Builds.Latest);
        var build = await HttpClient.GetFromJsonAsync<Build>(requestUriBuild, cancellationToken);
        
        if (build == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var downloadUri = string.Format(PurpurDownloadRequestUri, version.Version, build.BuildId);

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{build.BuildId}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await HttpClient
                .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

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
}