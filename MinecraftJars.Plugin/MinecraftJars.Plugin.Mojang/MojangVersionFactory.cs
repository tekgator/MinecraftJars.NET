using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mojang.Models.DetailApi;
using MinecraftJars.Plugin.Mojang.Models.ManifestApi;
using Group = MinecraftJars.Core.Versions.Group;

namespace MinecraftJars.Plugin.Mojang;

internal static partial class MojangVersionFactory
{
    public static IEnumerable<string> AvailableGameTypes => new List<string>
    {
        GameType.Vanilla,
        GameType.VanillaSnapshot,
        GameType.Bedrock,
        GameType.BedrockPreview
    };
    
    private const string MojangVanillaRequestUri = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

    [GeneratedRegex("(?i)https://minecraft\\.azureedge\\.net/bin-(?<platform>win|linux)(?:-)?(?<preview>preview)?/bedrock-server-(?<version>[0-9.]+).zip", RegexOptions.Compiled)]
    private static partial Regex MojangBedrockDownloadLink();
    private const string MojangBedrockRequestUri = "https://www.minecraft.net/download/server/bedrock";

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
        
        if ((options.Group is not null && options.Group is not Group.Server) ||
            (options.GameType is not null && options.GameType is not (GameType.Vanilla or GameType.VanillaSnapshot)))
        {
            return versions;
        }

        using var client = GetHttpClient();
        var manifest = await client.GetFromJsonAsync<Manifest>(MojangVanillaRequestUri, cancellationToken: cancellationToken) 
                       ?? new Manifest();

        if (options.Version is not null)
            manifest.Versions.RemoveAll(v => !v.Id.Equals(options.Version));

        versions.AddRange(manifest.Versions.Select(mojangVersion => new MojangVersion
        {
            Group = Group.Server,
            GameType = mojangVersion.Type == "release" ? GameType.Vanilla : GameType.VanillaSnapshot,
            Version = mojangVersion.Id,
            Os = Os.Windows | Os.Linux | Os.MacOs,
            ReleaseTime = mojangVersion.ReleaseTime,
            DetailUrl = mojangVersion.Url
        }));

        return versions;
    }

    private static async Task<List<MojangVersion>> GetBedrock(
        VersionOptions options, 
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<MojangVersion>();
        
        if (options.Group is not null && options.Group is not Group.Bedrock ||
            options.GameType is not null && options.GameType is not (GameType.Bedrock or GameType.BedrockPreview))
        {
            return versions;
        }
        
        using var client = GetHttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));        

        var response = await client.GetStringAsync(MojangBedrockRequestUri, cancellationToken);
        
        foreach (var match in MojangBedrockDownloadLink().Matches(response).Cast<Match>())
        {
            var url = match.Value;

            var gameType = match.Groups["preview"].Value.Length > 0 
                ? GameType.BedrockPreview 
                : GameType.Bedrock;
            
            if (options.GameType is not null && options.GameType != gameType)
                continue;

            var platform =
                match.Groups.Values.Where(p => p.Name == "platform").Select(p => p.Value).FirstOrDefault() switch
                {
                    "linux" => Os.Linux,
                    _ => Os.Windows
                };

            var version = match.Groups.Values.Where(p => p.Name == "version").Select(p => p.Value).First();
            
            if (options.Version is not null && !options.Version.Equals(version))
                continue;

            versions.Add(new MojangVersion
            {
                Group = Group.Server,
                GameType = gameType,
                Version = match.Groups.Values.Where(p => p.Name == "version").Select(p => p.Value).First(),
                Os = platform,
                DetailUrl = url
            });
        }

        return versions;
    }

    public static Task<IDownload> GetDownload(MojangVersion version)
    {
        return version.GameType is GameType.Bedrock or GameType.BedrockPreview 
            ? GetBedrockDownload(version.DetailUrl) 
            : GetVanillaDownload(version.DetailUrl);
    }
    
    private static async Task<IDownload> GetVanillaDownload(string detailUrl)
    {
        using var client = GetHttpClient();
        var detail = await client.GetFromJsonAsync<Detail>(detailUrl);

        if (detail is { Downloads.Server: not null })
        {
            return new MojangDownload
            {
                FileName = Path.GetFileName(new Uri(detail.Downloads.Server.Url).LocalPath),
                Size = detail.Downloads.Server.Size,
                Url = detail.Downloads.Server.Url,
                HashType = HashType.Sha1,
                Hash = detail.Downloads.Server.Sha1
            };
        }

        throw new InvalidOperationException("Could not acquire download details.");
    }
    
    private static async Task<IDownload> GetBedrockDownload(string detailUrl)
    {
        using var client = GetHttpClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, detailUrl);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        if (httpResponse.IsSuccessStatusCode)
            contentLength = httpResponse.Content.Headers.ContentLength ?? 0;

        return new MojangDownload
        {
            FileName = Path.GetFileName(new Uri(detailUrl).LocalPath),
            Size = contentLength,
            Url = detailUrl,
            HashType = HashType.None,
        };
    }    
    
    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();

        if (client.DefaultRequestHeaders.UserAgent.Any()) 
            return client;
        
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);

        return client;
    }    
}