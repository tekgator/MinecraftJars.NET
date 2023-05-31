using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur.Model;

public record PurpurVersion(
    IMinecraftProject Project,
    string Version,
    bool IsSnapShot = false) : IMinecraftVersion
{
    public Task<IMinecraftDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        PurpurVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}