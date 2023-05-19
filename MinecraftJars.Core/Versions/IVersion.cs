using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Core.Versions;

public interface IVersion
{
    string Group { get; }
    string GameType { get; }
    string Version { get; }
    Os Os { get; }
    DateTime? ReleaseTime { get; }
    Task<IDownload> GetDownload();
}
