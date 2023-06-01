using System.Net.Http.Json;
using MinecraftJars.Core;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Paper.Model;
using MinecraftJars.Plugin.Paper.Model.PaperApi;

namespace MinecraftJars.Plugin.Paper;

internal static class PaperVersionFactory
{
    private const string PaperProjectRequestUri = "https://api.papermc.io/v2/projects/{0}";
    private const string PaperBuildRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds";
    private const string PaperDownloadRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds/{2}/downloads/{3}";

    public static PluginHttpClientFactory HttpClientFactory { get; set; } = default!;
    
    public static async Task<List<PaperVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = PaperProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var client = HttpClientFactory.GetClient();        
        var projectApi = await client
            .GetFromJsonAsync<Project>(string.Format(PaperProjectRequestUri, project.Name.ToLower()), cancellationToken) ??
            throw new InvalidOperationException("Could not acquire version details.");                         
        
        projectApi.Versions.Reverse();

        var versions = (from version in projectApi.Versions
            where string.IsNullOrWhiteSpace(options.Version) || version.Equals(options.Version)
            let isSnapshot = version.Contains("pre", StringComparison.OrdinalIgnoreCase) ||
                             version.Contains("snapshot", StringComparison.OrdinalIgnoreCase)
            where options.IncludeSnapshotBuilds || !isSnapshot                             
            select new PaperVersion(
                Project: project,
                Version: version,
                IsSnapShot: isSnapshot

            )).ToList();
                
        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static async Task<IMinecraftDownload> GetDownload(
        DownloadOptions options, 
        PaperVersion version,
        CancellationToken cancellationToken)
    {
        var client = HttpClientFactory.GetClient();
        var requestUri = string.Format(PaperBuildRequestUri, version.Project.Name.ToLower(), version.Version);
        var detail = await client.GetFromJsonAsync<BuildVersions>(requestUri, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire download details.");
        
        var build = detail.Builds.Last();
            
        var downloadUri = string.Format(PaperDownloadRequestUri,
            version.Project.Name.ToLower(), version.Version, build.BuildId.ToString(), build.Downloads.Application.Name);

        long contentLength = 0;
        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await client
                .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
        }

        return new PaperDownload(
            FileName: build.Downloads.Application.Name,
            Size: contentLength,
            BuildId: build.BuildId.ToString(),
            Url: downloadUri,
            ReleaseTime: build.Time,
            HashType: HashType.Sha256,
            Hash: build.Downloads.Application.Sha256);
    }
}
