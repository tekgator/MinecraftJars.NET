using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang;

public class MojangVersion : IVersion
{
    public required string Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public required Os Os { get; init; }
    public DateTime? ReleaseTime { get; init; }
    internal string DetailUrl { get; init; } = string.Empty;
    
    public Task<IDownload> GetDownload()
    {
        return MojangVersionFactory.GetDownload(this);
    }
}