using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistVersion(
    IMinecraftProject Project,
    string Version,
    bool IsSnapShot = false) : IMinecraftVersion
{
    public Task<IMinecraftDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        MohistVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}
