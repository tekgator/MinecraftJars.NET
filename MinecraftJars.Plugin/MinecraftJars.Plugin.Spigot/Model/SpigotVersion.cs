using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotVersion(
    IProject Project,
    string Version,
    bool RequiresLocalBuild = false,
    bool IsSnapShot = false) : IVersion
{
    internal DateTime? ReleaseTime { get; init; }
    internal string DetailUrl { get; init; } = string.Empty;
    public Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) => 
        SpigotVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}