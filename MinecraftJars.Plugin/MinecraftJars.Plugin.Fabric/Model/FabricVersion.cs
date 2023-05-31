using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Fabric.Model;

public record FabricVersion(
    IMinecraftProject Project,
    string Version,
    bool IsSnapShot) : IMinecraftVersion
{
    internal string InstallerVersion { get; init; } = string.Empty;
    
    public Task<IMinecraftDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        FabricVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}
