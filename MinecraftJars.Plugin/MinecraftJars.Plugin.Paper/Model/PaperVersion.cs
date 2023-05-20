using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper.Model;

public class PaperVersion : IVersion
{
    public required Group Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public Task<IDownload> GetDownload()
    {
        return PaperVersionFactory.GetDownload(this);
    }
}