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
        
        var releaseApi = await HttpClient.GetFromJsonAsync<List<Release>>(PocketmineReleaseRequestUri, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire version details.");        

        foreach (var release in releaseApi)
        {
            release.Assets.RemoveAll(a =>
                !a.Name.Contains(".phar", StringComparison.OrdinalIgnoreCase) || 
                !a.State.Equals("uploaded", StringComparison.OrdinalIgnoreCase));
        }
        releaseApi.RemoveAll(r => !r.Assets.Any());

        var versions = (from release in releaseApi 
            where string.IsNullOrWhiteSpace(options.Version) || release.TagName.Equals(options.Version)
            let isSnapShot = release.TagName.Contains("beta", StringComparison.OrdinalIgnoreCase) ||
                             release.TagName.Contains("alpha", StringComparison.OrdinalIgnoreCase)
            where options.IncludeSnapshotBuilds || !isSnapShot
            let asset = release.Assets.First()
            select new PocketmineVersion(
                Project: project, 
                Version: release.TagName, 
                IsSnapShot: isSnapShot)
            {
                Download = new PocketmineDownload(
                    FileName: asset.Name, 
                    Size: asset.Size, 
                    BuildId: release.Id.ToString(), 
                    Url: asset.BrowserDownloadUrl, 
                    ReleaseTime: new[] { asset.CreatedAt, asset.UpdatedAt }.Max())
            }).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }
    
    public static Task<IMinecraftDownload> GetDownload(
        DownloadOptions options, 
        PocketmineVersion version,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IMinecraftDownload) version.Download);
    }      
}