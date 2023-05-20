using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public class MohistVersion : IVersion
{
    public required string Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public required Os Os { get; init; }
    public DateTime? ReleaseTime { get; internal set; }
    public Task<IDownload> GetDownload()
    {
        return MohistVersionFactory.GetDownload(this);
    }
}