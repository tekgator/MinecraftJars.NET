using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Core.Versions;

public interface IVersion
{
    Group Group { get; }
    string GameType { get; }
    string Version { get; }
    Task<IDownload> GetDownload();
}
