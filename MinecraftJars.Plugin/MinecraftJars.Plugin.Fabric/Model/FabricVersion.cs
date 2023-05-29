using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Fabric.Model;

public record FabricVersion(
    IProject Project,
    string Version) : IVersion
{
    internal string InstallerVersion { get; init; } = string.Empty;
    
    public Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        FabricVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}
