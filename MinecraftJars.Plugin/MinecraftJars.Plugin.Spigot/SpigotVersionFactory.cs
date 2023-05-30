using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.RegularExpressions;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Spigot.Model;
using MinecraftJars.Plugin.Spigot.Model.JenkinsApi;
using MinecraftJars.Plugin.Spigot.Model.SpigotApi;
using Group = MinecraftJars.Core.Projects.Group;

namespace MinecraftJars.Plugin.Spigot;

internal static partial class SpigotVersionFactory
{
    private const string SpigotRequestUri = "https://hub.spigotmc.org/versions";
    [GeneratedRegex("(?im)<a href=\"(?<json>(?<version>1\\.[a-z0-9.-]+).json)\">*(.+)(?<date>\\d{2}-\\w{3}-\\d{4} [0-9:]+)", RegexOptions.Compiled)]
    private static partial Regex SpigotVersions();

    private const string BungeeCoordRequestUri = "https://ci.md-5.net/job/BungeeCord/api/json?tree=builds[number,url,result,inProgress,timestamp,artifacts[fileName,relativePath]]";
    private const string BungeeCoordRequestUriMaxRecordSuffix = "{{0,{0}}}";
    
    public static HttpClient HttpClient { get; set; } = default!;
    
    public static Task<List<SpigotVersion>> GetVersion(
        string projectName,
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        return SpigotProjectFactory.Projects.SingleOrDefault(p => p.Name.Equals(projectName))?.Group switch
        {
            Group.Server => GetVersionSpigot(projectName, options, cancellationToken),
            Group.Proxy => GetVersionBungeeCoord(projectName, options, cancellationToken),
            _ => throw new InvalidOperationException("Could not acquire version details.")
        };
    }

    private static async Task<List<SpigotVersion>> GetVersionSpigot(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = SpigotProjectFactory.Projects.Single(p => p.Name.Equals(projectName));
        
        var request = new HttpRequestMessage(HttpMethod.Get, SpigotRequestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
        request.Headers.AcceptEncoding.ParseAdd("identity");
        request.Headers.AcceptLanguage.ParseAdd("en-US, en");
        request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        
        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Could not acquire version details.");

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        var versions = (from match in SpigotVersions().Matches(html)
            let version = match.Groups["version"].Value
            let jsonFile = match.Groups["json"].Value
            let releaseTime = DateTime.Parse(match.Groups["date"].Value, new CultureInfo("en-US"))
            where !string.IsNullOrWhiteSpace(version) &&
                  (string.IsNullOrWhiteSpace(options.Version) || options.Version.Equals(version))
            orderby releaseTime descending
            select new SpigotVersion(
                Project: project,
                Version: version,
                RequiresLocalBuild: true)
            {
                DetailUrl = $"{SpigotRequestUri}/{jsonFile}",
                ReleaseTime = releaseTime
            }).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    private static async Task<List<SpigotVersion>> GetVersionBungeeCoord(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = SpigotProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var requestUrl = BungeeCoordRequestUri + (options.MaxRecords.HasValue
            ? string.Format(BungeeCoordRequestUriMaxRecordSuffix, options.MaxRecords.Value)
            : string.Empty);
        
        var job = await HttpClient.GetFromJsonAsync<Job>(requestUrl, cancellationToken);
            
        if (job == null)
            throw new InvalidOperationException("Could not acquire version details.");

        var versions = (from build in job.Builds
            where !build.InProgress && 
                  build.Result.Equals("success", StringComparison.OrdinalIgnoreCase)
            let version = build.Number.ToString()
            let artifact = build.Artifacts.First()
            where string.IsNullOrWhiteSpace(options.Version) || options.Version.Equals(build.Number.ToString())
            select new SpigotVersion(
                Project: project,
                Version: version)
            {
                ReleaseTime = DateTimeOffset.FromUnixTimeMilliseconds(build.Timestamp).DateTime,
                DetailUrl = $"{build.Url}artifact/{artifact.RelativePath}"
            }).ToList();
        
        return versions;
    }

    public static Task<IDownload> GetDownload(
        DownloadOptions options, 
        SpigotVersion version,
        CancellationToken cancellationToken)
    {
        return version.Project.Group == Group.Server 
            ? GetDownloadSpigot(options, version, cancellationToken) 
            : GetDownloadBungeeCord(options, version, cancellationToken);
    }
    
    private static async Task<IDownload> GetDownloadSpigot(
        DownloadOptions options, 
        SpigotVersion version,
        CancellationToken cancellationToken = default!)
    {
        var build = await HttpClient.GetFromJsonAsync<SpigotBuild>(version.DetailUrl, cancellationToken);

        if (build == null) 
            throw new InvalidOperationException("Could not acquire download details.");

        if (!options.BuildJar)
            return new SpigotDownload(
                FileName: string.Empty,
                Size: 0,
                BuildId: build.Name,
                Url: string.Empty,
                ReleaseTime: version.ReleaseTime);

        var buildTool = new SpigotBuildTools(HttpClient, options, version);
        return await buildTool.Build(build.Name,cancellationToken);
    }
    
    private static async Task<IDownload> GetDownloadBungeeCord(
        DownloadOptions options, 
        SpigotVersion version,
        CancellationToken cancellationToken)
    {
        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}.jar";
        
        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
            using var httpResponse = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? 0;
        }

        return new SpigotDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: version.Version,
            Url: version.DetailUrl,
            ReleaseTime: version.ReleaseTime);
    }
}