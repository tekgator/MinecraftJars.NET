using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotVersion(
    IProject Project,
    string Version) : IVersion
{
    internal DateTime? ReleaseTime { get; init; }
    internal string? DetailUrl { get; init; }
    public Task<IDownload> GetDownload(DownloadOptions? options = null) => 
        SpigotVersionFactory.GetDownload(options ?? new DownloadOptions(), this);
}