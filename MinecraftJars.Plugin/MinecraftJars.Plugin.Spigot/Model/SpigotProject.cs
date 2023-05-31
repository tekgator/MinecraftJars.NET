using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotProject(
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
        return await SpigotVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }    
}