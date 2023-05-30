using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang.Models;

public record MojangVersion(
    IProject Project,
    string Version,
    bool IsSnapShot,
    Os? Os = null) : IVersion
{
    internal DateTime? ReleaseTime { get; init; }
    internal string DetailUrl { get; init; } = string.Empty;
    
    public Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!) =>
        MojangVersionFactory.GetDownload(options ?? new DownloadOptions(), this, cancellationToken);
}