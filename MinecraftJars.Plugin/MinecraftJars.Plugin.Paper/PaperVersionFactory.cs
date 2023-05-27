using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Paper.Model;
using MinecraftJars.Plugin.Paper.Model.BuildApi;
using MinecraftJars.Plugin.Paper.Model.ProjectApi;

namespace MinecraftJars.Plugin.Paper;

internal static class PaperVersionFactory
{
    private const string PaperProjectRequestUri = "https://api.papermc.io/v2/projects/{0}";
    private const string PaperBuildRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds";
    private const string PaperDownloadRequestUri = "https://api.papermc.io/v2/projects/{0}/versions/{1}/builds/{2}/downloads/{3}";

    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
    public static async Task<List<PaperVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var versions = new List<PaperVersion>();
        var project = PaperProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        using var client = GetHttpClient();

        var projectApi = await client
                          .GetFromJsonAsync<Project>(string
                              .Format(PaperProjectRequestUri, project.Name.ToLower()), cancellationToken);
        
        if (projectApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");

        if (!string.IsNullOrWhiteSpace(options.Version))
            projectApi.Versions.RemoveAll(v => !v.Equals(options.Version));
        
        projectApi.Versions.Reverse();
        versions.AddRange(projectApi.Versions
            .Select(projectVersion => new PaperVersion(
                Project: project,
                Version: projectVersion
            )));

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static async Task<IDownload> GetDownload(
        DownloadOptions options, 
        PaperVersion version,
        CancellationToken cancellationToken)
    {
        using var client = GetHttpClient();
        
        var requestUri = string.Format(PaperBuildRequestUri, version.Project.Name.ToLower(), version.Version);
        var detail = await client.GetFromJsonAsync<BuildVersions>(requestUri, cancellationToken);

        if (detail == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        var build = detail.Builds.Last();
            
        var downloadUri = string.Format(PaperDownloadRequestUri,
            version.Project.Name.ToLower(), version.Version, build.BuildId.ToString(), build.Downloads.Application.Name);

        long contentLength = 0;
        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUri);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

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
