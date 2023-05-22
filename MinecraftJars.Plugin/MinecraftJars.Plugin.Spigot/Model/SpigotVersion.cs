using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotVersion(
    IProject Project,
    string Version) : IVersion
{
    public Task<IDownload> GetDownload() => SpigotVersionFactory.GetDownload(this);
}