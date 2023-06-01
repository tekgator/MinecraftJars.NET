using System.Net.Http.Json;
using MinecraftJars.Core;
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
    
    public static PluginHttpClientFactory HttpClientFactory { get; set; } = default!;
    
    public static async Task<List<PurpurVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = PurpurProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var client = HttpClientFactory.GetClient();
        var projectApi = await client.GetFromJsonAsync<Project>(PurpurProjectRequestUri, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire version details.");
        
        projectApi.Versions.Reverse();
        var versions = (from version in projectApi.Versions
                where string.IsNullOrWhiteSpace(options.Version) || version.Equals(options.Version)
                select new PurpurVersion(
                    Project: project,
                    Version: version)
            ).ToList();
        
        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static async Task<IMinecraftDownload> GetDownload(
        DownloadOptions options, 
        PurpurVersion version,
        CancellationToken cancellationToken)
    {
        var client = HttpClientFactory.GetClient();

        var requestUriVersionBuilds = string.Format(PurpurVersionBuildsRequestUri, version.Version);
        var versionBuilds = await client.GetFromJsonAsync<VersionBuilds>(requestUriVersionBuilds, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire download details.");
        
        var requestUriBuild = string.Format(PurpurBuildRequestUri, version.Version, versionBuilds.Builds.Latest);
        var build = await client.GetFromJsonAsync<Build>(requestUriBuild, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire download details.");
        
        var downloadUri = string.Format(PurpurDownloadRequestUri, version.Version, build.BuildId);

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{build.BuildId}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await client
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