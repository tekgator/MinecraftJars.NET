using System.Net.Http.Json;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mohist.Model;
using MinecraftJars.Plugin.Mohist.Model.MohistApi;

namespace MinecraftJars.Plugin.Mohist;

internal static class MohistVersionFactory
{
    private const string MohistVersionRequestUri = "https://mohistmc.com/api/versions";
    private const string MohistLatestBuildRequestUri = "https://mohistmc.com/api/{0}/latest/";

    public static HttpClient HttpClient { get; set; } = default!;
    
    public static async Task<List<MohistVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = MohistProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var availVersions = await HttpClient.GetFromJsonAsync<List<string>>(MohistVersionRequestUri, cancellationToken);        
    
        if (availVersions == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
   
        if (!string.IsNullOrWhiteSpace(options.Version))
            availVersions.RemoveAll(v => !v.Equals(options.Version));
        
        availVersions.Reverse();

        var versions = availVersions
            .Select(availVersion => new MohistVersion(
                Project: project, 
                Version: availVersion)
            ).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    public static async Task<IDownload> GetDownload(
        DownloadOptions options, 
        MohistVersion version, 
        CancellationToken cancellationToken)
    {
        var requestUriLatestBuild = string.Format(MohistLatestBuildRequestUri, version.Version);
        var latestBuild = await HttpClient.GetFromJsonAsync<Build>(requestUriLatestBuild, cancellationToken);

        if (latestBuild == null || string.IsNullOrWhiteSpace(latestBuild.Url)) 
            throw new InvalidOperationException("Could not acquire download details.");

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{latestBuild.Number}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, latestBuild.Url);
            using var httpResponse = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
                fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
            }
        }

        return new MohistDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: latestBuild.Number.ToString(),
            Url: latestBuild.Url,
            ReleaseTime: DateTimeOffset.FromUnixTimeMilliseconds(latestBuild.Timeinmillis).DateTime,
            HashType: HashType.Md5,
            Hash: latestBuild.Md5
        );
    }
}