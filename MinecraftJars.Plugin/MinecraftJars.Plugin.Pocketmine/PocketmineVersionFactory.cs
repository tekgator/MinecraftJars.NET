using System.Net.Http.Json;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Pocketmine.Model;
using MinecraftJars.Plugin.Pocketmine.Model.ReleaseApi;

namespace MinecraftJars.Plugin.Pocketmine;

internal static class PocketmineVersionFactory
{
    private const string PocketmineReleaseRequestUri = "https://api.github.com/repos/pmmp/PocketMine-MP/releases";
    
    public static HttpClient HttpClient { get; set; } = default!;
    
    public static async Task<List<PocketmineVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = PocketmineProjectFactory.Projects.Single(p => p.Name.Equals(projectName));
        
        var releaseApi = await HttpClient.GetFromJsonAsync<List<Release>>(PocketmineReleaseRequestUri, cancellationToken);
        
        if (releaseApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");

        if (!string.IsNullOrWhiteSpace(options.Version))
            releaseApi.RemoveAll(v => !v.TagName.Equals(options.Version));
            
        foreach (var release in releaseApi)
        {
            release.Assets.RemoveAll(a =>
                !a.Name.Contains(".phar", StringComparison.OrdinalIgnoreCase) || 
                !a.State.Equals("uploaded", StringComparison.OrdinalIgnoreCase));
        }
        releaseApi.RemoveAll(r => !r.Assets.Any());

        var versions = releaseApi
            .Select(release => new { release, asset = release.Assets.First() })
            .Select(t => new PocketmineVersion(
                Project: project, 
                Version: t.release.TagName)
            {
                Download = new PocketmineDownload(
                    FileName: t.asset.Name, 
                    Size: t.asset.Size,
                    BuildId: t.release.Id.ToString(), 
                    Url: t.asset.BrowserDownloadUrl,
                    ReleaseTime: new[] { t.asset.CreatedAt, t.asset.UpdatedAt }.Max())
            }).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static Task<IDownload> GetDownload(
        DownloadOptions options, 
        PocketmineVersion version,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IDownload) version.Download);
    }      
}