using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang.Models;

public class MojangVersion : IVersion
{
    public required Group Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public Os? Os { get; init; }
    internal DateTime? ReleaseTime { get; init; }
    internal string DetailUrl { get; init; } = string.Empty;
    
    public Task<IDownload> GetDownload()
    {
        return MojangVersionFactory.GetDownload(this);
    }
}