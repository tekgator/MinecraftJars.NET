using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public class MohistVersion : IVersion
{
    public required Group Group { get; init; }
    public required string GameType { get; init; }
    public required string Version { get; init; }
    public Task<IDownload> GetDownload()
    {
        return MohistVersionFactory.GetDownload(this);
    }
}