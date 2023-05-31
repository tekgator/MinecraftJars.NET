using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.RegularExpressions;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mojang.Models;
using MinecraftJars.Plugin.Mojang.Models.MojangApi;
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
        var project = MojangProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var manifest = await HttpClient.GetFromJsonAsync<Manifest>(MojangVanillaRequestUri, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire version details.");
        
        var versions = (from version in manifest.Versions
            let isSnapshot = !version.Type.Equals("release", StringComparison.OrdinalIgnoreCase)
            where string.IsNullOrWhiteSpace(options.Version) || version.Id.Equals(options.Version)
            where options.IncludeSnapshotBuilds || !isSnapshot
            select new MojangVersion(
                Project: project,
                Version: version.Id,
                IsSnapShot: isSnapshot)
                {
                    ReleaseTime = version.ReleaseTime,
                    DetailUrl = version.Url
                }).ToList();
       
        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    private static async Task<List<MojangVersion>> GetVersionBedrock(
        string projectName,
        VersionOptions options, 
        CancellationToken cancellationToken)
    {
        var project = MojangProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var request = new HttpRequestMessage(HttpMethod.Get, MojangBedrockRequestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
        request.Headers.AcceptEncoding.ParseAdd("identity");
        request.Headers.AcceptLanguage.ParseAdd("en-US, en");
        request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Could not acquire version details.");

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        var versions = (from match in MojangBedrockDownloadLink().Matches(html)
            let version = match.Groups["version"].Value
            where string.IsNullOrWhiteSpace(options.Version) || version.Equals(options.Version)            
            let isPreview = !string.IsNullOrWhiteSpace(match.Groups["preview"].Value)
            where options.IncludeSnapshotBuilds || !isPreview
            let url = match.Value
            let platform = match.Groups["platform"].Value.Equals("linux", StringComparison.OrdinalIgnoreCase) ? Os.Linux : Os.Windows
            select new MojangVersion(
                Project: project, 
                Version: version, 
                IsSnapShot: isPreview, 
                Os: platform)
            {
                DetailUrl = url
            }).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    public static Task<IMinecraftDownload> GetDownload(
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
    
    private static async Task<IMinecraftDownload> GetDownloadVanilla(
        DownloadOptions options, 
        MojangVersion version,
        CancellationToken cancellationToken)
    {
        var detail = await HttpClient.GetFromJsonAsync<Detail>(version.DetailUrl, cancellationToken) ??
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
    
    private static async Task<IMinecraftDownload> GetDownloadBedrock(
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