using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;

namespace MinecraftJars.Core.Versions;

public interface IVersion
{
    IProject Project { get; }
    string Version { get; }
    bool RequiresLocalBuild => false;
    Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!);
}
