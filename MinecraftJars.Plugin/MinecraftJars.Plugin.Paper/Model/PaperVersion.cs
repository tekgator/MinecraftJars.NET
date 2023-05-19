using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper.Model;

public class PaperVersion : IVersion
{
    public required string Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public required Os Os { get; init; }
    public DateTime? ReleaseTime { get; internal set; }
    public Task<IDownload> GetDownload()
    {
        return PaperVersionFactory.GetDownload(this);
    }
}