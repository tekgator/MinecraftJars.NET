using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper.Model;

public class PaperVersion : IVersion
{
    public required IProject Project { get; init; }
    public required string Version { get; init; }
    public Task<IDownload> GetDownload()
    {
        return PaperVersionFactory.GetDownload(this);
    }
}