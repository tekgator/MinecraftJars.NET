using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur.Model;

public record PurpurVersion(
    IProject Project,
    string Version) : IVersion
{
    public Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        PurpurVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}