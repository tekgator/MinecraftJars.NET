using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mojang.Models;
using MinecraftJars.Plugin.Mojang.Models.DetailApi;
using MinecraftJars.Plugin.Mojang.Models.ManifestApi;
using Group = MinecraftJars.Core.Projects.Group;

namespace MinecraftJars.Plugin.Mojang;

internal static partial class MojangVersionFactory
{
    private const string MojangVanillaRequestUri = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

    [GeneratedRegex("(?i)https://minecraft\\.azureedge\\.net/bin-(?<platform>win|linux)(?:-)?(?<preview>preview)?/bedrock-server-(?<version>[0-9.]+).zip", RegexOptions.Compiled)]
    private static partial Regex MojangBedrockDownloadLink();
    private const string MojangBedrockRequestUri = "https://www.minecraft.net/download/server/bedrock";

    public static IHttpClientFactory? HttpClientFactory { get; set; }
    
    public static async Task<List<MojangVersion>> Get(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var taskVanilla = GetVanilla(options, cancellationToken);
        var taskBedrock = GetBedrock(options, cancellationToken);

        await Task.WhenAll(taskVanilla, taskBedrock);

        return (await taskVanilla).Concat(await taskBedrock).ToList();
    }
    
    private static async Task<List<MojangVersion>> GetVanilla(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<MojangVersion>();
        var projects = new List<MojangProject>(MojangProjectFactory.Projects);

        projects.RemoveAll(p => p.Group != Group.Server);
        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));        

        if (!projects.Any() || (options.Group is not null && options.Group is not Group.Server))
            return versions;
        
        using var client = GetHttpClient();
        var manifest = await client.GetFromJsonAsync<Manifest>(MojangVanillaRequestUri, cancellationToken);

        if (manifest == null)
            throw new InvalidOperationException("Could not acquire version details.");
        
        if (options.Version is not null)
            manifest.Versions.RemoveAll(v => !v.Id.Equals(options.Version));

        foreach (var project in projects.Where(p => p.Group == Group.Server))
        {
            versions.AddRange(manifest.Versions
                .Where(v => project.Name == MojangProjectFactory.Vanilla ? 
                    v.Type.Equals("release", StringComparison.OrdinalIgnoreCase) : 
                    !v.Type.Equals("release", StringComparison.OrdinalIgnoreCase))
                .Select(version => new MojangVersion(
                    Project: project, 
                    Version: version.Id) {
                    ReleaseTime = version.ReleaseTime, 
                    DetailUrl = version.Url
                }));
        }
        
        return versions;
    }

    private static async Task<List<MojangVersion>> GetBedrock(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<MojangVersion>();
        var projects = new List<MojangProject>(MojangProjectFactory.Projects);

        projects.RemoveAll(p => p.Group != Group.Bedrock);
        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));        

        if (!projects.Any() || (options.Group is not null && options.Group is not Group.Bedrock))
            return versions;
        
        using var client = GetHttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));        

        var response = await client.GetStringAsync(MojangBedrockRequestUri, cancellationToken);
        
        foreach (var match in MojangBedrockDownloadLink().Matches(response).Cast<Match>())
        {
            var url = match.Value;
            
            var version = match.Groups.Values
                .Where(p => p.Name == "version")
                .Select(p => p.Value)
                .FirstOrDefault();
            
            if (version == null)
                continue;
            
            if (options.Version is not null && !options.Version.Equals(version))
                continue;

            var project = match.Groups["preview"].Value.Length > 0 
                ? projects.FirstOrDefault(p => p.Name.Equals(MojangProjectFactory.BedrockPreview)) 
                : projects.FirstOrDefault(p => p.Name.Equals(MojangProjectFactory.Bedrock));
            
            if (project == null)
                continue;

            var platform = match.Groups.Values
                    .Where(p => p.Name == "platform")
                    .Select(p => p.Value)
                    .FirstOrDefault() switch 
                {
                    "linux" => Os.Linux,
                    _ => Os.Windows
                };

            versions.Add(new MojangVersion(
                Project: project,
                Version: version,
                Os: platform) {
                DetailUrl = url
            });
        }

        return versions;
    }

    public static Task<IDownload> GetDownload(MojangVersion version)
    {
        return version.Project.Group == Group.Bedrock 
            ? GetBedrockDownload(version) 
            : GetVanillaDownload(version);
    }
    
    private static async Task<IDownload> GetVanillaDownload(MojangVersion version)
    {
        using var client = GetHttpClient();
        var detail = await client.GetFromJsonAsync<Detail>(version.DetailUrl);

        if (detail is { Downloads.Server: not null })
        {
            return new MojangDownload(
                FileName: Path.GetFileName(new Uri(detail.Downloads.Server.Url).LocalPath),
                Size: detail.Downloads.Server.Size,
                Url: detail.Downloads.Server.Url,
                ReleaseTime: version.ReleaseTime,
                HashType: HashType.Sha1,
                Hash: detail.Downloads.Server.Sha1);
        }

        throw new InvalidOperationException("Could not acquire download details.");
    }
    
    private static async Task<IDownload> GetBedrockDownload(MojangVersion version)
    {
        using var client = GetHttpClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        if (httpResponse.IsSuccessStatusCode)
            contentLength = httpResponse.Content.Headers.ContentLength ?? 0;

        return new MojangDownload(
            FileName: Path.GetFileName(new Uri(version.DetailUrl).LocalPath),
            Size: contentLength,
            Url: version.DetailUrl);
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