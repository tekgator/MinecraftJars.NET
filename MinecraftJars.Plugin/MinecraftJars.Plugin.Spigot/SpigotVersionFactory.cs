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
    
    public static async Task<List<SpigotVersion>> Get(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var taskSpigot = GetSpigot(options, cancellationToken);
        //var taskBungeeCoord = GetBungeeCoord(options, cancellationToken);
        var taskBungeeCoord = Task.FromResult(new List<SpigotVersion>());

        await Task.WhenAll(taskSpigot, taskBungeeCoord);

        return (await taskSpigot).Concat(await taskBungeeCoord).ToList();
    }

    private static async Task<List<SpigotVersion>> GetSpigot(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<SpigotVersion>();
        var projects = new List<SpigotProject>(SpigotProjectFactory.Projects);
        
        projects.RemoveAll(p => p.Group != Group.Server);
        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));        

        if (!projects.Any() || options.Group is not null && options.Group is not Group.Server)
            return versions;

        var project = projects.FirstOrDefault(p => p.Name.Equals(SpigotProjectFactory.Spigot));
        if (project == null)
            return versions;
        
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
                Version: match.Groups["version"].Value)
            {
                DetailUrl = $"{SpigotRequestUri}/{match.Groups["json"].Value}",
                ReleaseTime = DateTime.Parse(match.Groups["date"].Value, new CultureInfo("en-US"))
            })            
            .OrderByDescending(v => v.ReleaseTime));

        return versions;
    }

    private static async Task<List<SpigotVersion>> GetBungeeCoord(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<SpigotVersion>();
        var projects = new List<SpigotProject>(SpigotProjectFactory.Projects);

        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));

        if (!projects.Any() || options.Group is not null && options.Group is not Group.Proxy)
            return versions;

        var project = projects.FirstOrDefault(p => p.Name.Equals(SpigotProjectFactory.BungeeCord));
        if (project == null)
            return versions;
        
        using var client = GetHttpClient();

        var requestUrl = BungeeCoordRequestUri + (options.MaxRecords == null
            ? string.Empty
            : string.Format(BungeeCoordRequestUriMaxRecordSuffix, options.MaxRecords));
        
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
    
    public static async Task<IDownload> GetDownload(DownloadOptions options, SpigotVersion version)
    {
        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}.jar";
        
        if (options.LoadFilesize)
        {
            using var client = GetHttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? 0;
        }

        return new SpigotDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: int.Parse(version.Version),
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