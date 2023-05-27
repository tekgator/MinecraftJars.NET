using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
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
    
    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
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
        var versions = new List<SpigotVersion>();
        var project = SpigotProjectFactory.Projects.Single(p => p.Name.Equals(projectName));
        
        var request = new HttpRequestMessage(HttpMethod.Get, SpigotRequestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

        using var client = GetHttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Could not acquire version details.");

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        versions.AddRange(SpigotVersions()
            .Matches(html)
            .Select(match => new SpigotVersion(
                Project: project,
                Version: match.Groups["version"].Value,
                RequiresLocalBuild: true)
            {
                DetailUrl = $"{SpigotRequestUri}/{match.Groups["json"].Value}",
                ReleaseTime = DateTime.Parse(match.Groups["date"].Value, new CultureInfo("en-US"))
            })            
            .OrderByDescending(v => v.ReleaseTime));

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    private static async Task<List<SpigotVersion>> GetVersionBungeeCoord(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var versions = new List<SpigotVersion>();
        var project = SpigotProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        using var client = GetHttpClient();

        var requestUrl = BungeeCoordRequestUri + (options.MaxRecords.HasValue
            ? string.Format(BungeeCoordRequestUriMaxRecordSuffix, options.MaxRecords.Value)
            : string.Empty);
        
        var job = await client.GetFromJsonAsync<Job>(requestUrl, cancellationToken);
            
        if (job == null)
            throw new InvalidOperationException("Could not acquire version details.");

        versions.AddRange(job.Builds
            .Where(b => !b.InProgress && 
                        b.Result.Equals("success", StringComparison.OrdinalIgnoreCase))
            .Select(b => new SpigotVersion(
                project, 
                b.Number.ToString())
                {
                    ReleaseTime = DateTimeOffset.FromUnixTimeMilliseconds(b.Timestamp).DateTime, 
                    DetailUrl = $"{b.Url}artifact/{b.Artifacts.First().RelativePath}"
                }));
        
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
        using var client = GetHttpClient();
        var build = await client.GetFromJsonAsync<SpigotBuild>(version.DetailUrl, cancellationToken);

        if (build == null) 
            throw new InvalidOperationException("Could not acquire download details.");

        if (!options.BuildJar)
            return new SpigotDownload(
                FileName: string.Empty,
                Size: 0,
                BuildId: build.Name,
                Url: string.Empty,
                ReleaseTime: version.ReleaseTime);

        var buildTool = new SpigotBuildTools(GetHttpClient(), options, version);
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
            using var client = GetHttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? 0;
        }

        return new SpigotDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: version.Version,
            Url: version.DetailUrl!,
            ReleaseTime: version.ReleaseTime);
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