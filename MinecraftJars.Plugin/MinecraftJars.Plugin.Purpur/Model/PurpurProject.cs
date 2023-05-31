using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur.Model;

public record PurpurProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject
{
    public async Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null,
        CancellationToken cancellationToken = default!)
    {
        return await PurpurVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }     
}