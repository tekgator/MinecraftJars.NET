using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur.Model;

public class PurpurVersion : IVersion
{
    public required string Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public Task<IDownload> GetDownload()
    {
        return PurpurVersionFactory.GetDownload(this);
    }
}