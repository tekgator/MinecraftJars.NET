using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Core.Versions;

public interface IVersion
{
    string Group { get; }
    string GameType { get; }
    string Version { get; }
    Task<IDownload> GetDownload();
}
