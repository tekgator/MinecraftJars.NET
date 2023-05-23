using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper.Model;

public record PaperVersion(
    IProject Project,
    string Version) : IVersion
{
    public Task<IDownload> GetDownload(DownloadOptions? options = null) => 
        PaperVersionFactory.GetDownload(options ?? new DownloadOptions(), this);
}