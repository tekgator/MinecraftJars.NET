using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
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

    public static HttpClient HttpClient { get; set; } = default!;
    
    public static Task<List<MojangVersion>> GetVersion(
        string projectName,
        VersionOptions options, 
        CancellationToken cancellationToken)
    {
        return MojangProjectFactory.Projects.SingleOrDefault(p => p.Name.Equals(projectName))?.Group switch
        {
            Group.Bedrock => GetVersionBedrock(projectName, options, cancellationToken),
            Group.Server => GetVersionVanilla(projectName, options, cancellationToken),
            _ => throw new InvalidOperationException("Could not acquire version details.")
        };
    }
    
    private static async Task<List<MojangVersion>> GetVersionVanilla(
        string projectName,
        VersionOptions options, 
        CancellationToken cancellationToken)
    {
        var versions = new List<MojangVersion>();
        var project = MojangProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var manifest = await HttpClient.GetFromJsonAsync<Manifest>(MojangVanillaRequestUri, cancellationToken);

        if (manifest == null)
            throw new InvalidOperationException("Could not acquire version details.");
        
        if (!string.IsNullOrWhiteSpace(options.Version))
            manifest.Versions.RemoveAll(v => !v.Id.Equals(options.Version));

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
        
        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    private static async Task<List<MojangVersion>> GetVersionBedrock(
        string projectName,
        VersionOptions options, 
        CancellationToken cancellationToken)
    {
        var versions = new List<MojangVersion>();
        var project = MojangProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var request = new HttpRequestMessage(HttpMethod.Get, MojangBedrockRequestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Could not acquire version details.");

        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        
        foreach (var match in MojangBedrockDownloadLink().Matches(html).Cast<Match>())
        {
            var url = match.Value;
            
            var version = match.Groups.Values
                .Where(p => p.Name == "version")
                .Select(p => p.Value)
                .FirstOrDefault();
            
            if (version == null)
                continue;
            
            if (!string.IsNullOrWhiteSpace(options.Version) && !options.Version.Equals(version))
                continue;

            var isPreview = match.Groups["preview"].Value.Length > 0;
            
            if ((project.Name == MojangProjectFactory.Bedrock && isPreview) ||
                (project.Name == MojangProjectFactory.BedrockPreview && !isPreview))
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

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    public static Task<IDownload> GetDownload(
        DownloadOptions options, 
        MojangVersion version, 
        CancellationToken cancellationToken)
    {
        return version.Project.Group switch
        {
            Group.Bedrock => GetDownloadBedrock(options, version, cancellationToken),
            Group.Server => GetDownloadVanilla(options, version, cancellationToken),
            _ => throw new InvalidOperationException("Could not acquire download details.")
        };
    }
    
    private static async Task<IDownload> GetDownloadVanilla(
        DownloadOptions options, 
        MojangVersion version,
        CancellationToken cancellationToken)
    {
        var detail = await HttpClient.GetFromJsonAsync<Detail>(version.DetailUrl, cancellationToken);

        if (detail == null) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        if (detail.Downloads.Server == null)
        {
            return new MojangDownload(
                FileName: string.Empty,
                Size: 0,
                BuildId: version.Version,
                Url: string.Empty,
                ReleaseTime: version.ReleaseTime,
                HashType: HashType.None);
        }

        return new MojangDownload(
            FileName: Path.GetFileName(new Uri(detail.Downloads.Server.Url).LocalPath),
            Size: detail.Downloads.Server.Size,
            BuildId: version.Version,
            Url: detail.Downloads.Server.Url,
            ReleaseTime: version.ReleaseTime,
            HashType: HashType.Sha1,
            Hash: detail.Downloads.Server.Sha1);     
    }
    
    private static async Task<IDownload> GetDownloadBedrock(
        DownloadOptions options, 
        MojangVersion version,
        CancellationToken cancellationToken)
    {
        long contentLength = 0;
        
        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, version.DetailUrl);
            using var httpResponse = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
                contentLength = httpResponse.Content.Headers.ContentLength ?? 0;
        }

        return new MojangDownload(
            FileName: Path.GetFileName(new Uri(version.DetailUrl).LocalPath),
            Size: contentLength,
            BuildId: version.Version,
            Url: version.DetailUrl);
    }
}