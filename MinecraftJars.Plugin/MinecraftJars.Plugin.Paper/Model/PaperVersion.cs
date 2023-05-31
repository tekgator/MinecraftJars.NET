using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper.Model;

public record PaperVersion(
    IMinecraftProject Project,
    string Version,
    bool IsSnapShot) : IMinecraftVersion
{
    public Task<IMinecraftDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        PaperVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}