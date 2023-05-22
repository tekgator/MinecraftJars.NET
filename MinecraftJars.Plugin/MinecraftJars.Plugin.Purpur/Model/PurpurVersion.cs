using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur.Model;

public class PurpurVersion : IVersion
{
    public required IProject Project { get; init; }
    public required string Version { get; init; }
    public Task<IDownload> GetDownload()
    {
        return PurpurVersionFactory.GetDownload(this);
    }
}