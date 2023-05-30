using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Pocketmine.Model;

public record PocketmineVersion(
    IProject Project,
    string Version,
    bool IsSnapShot) : IVersion
{
    internal PocketmineDownload Download { get; init; } = default!;
    
    public Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        PocketmineVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}