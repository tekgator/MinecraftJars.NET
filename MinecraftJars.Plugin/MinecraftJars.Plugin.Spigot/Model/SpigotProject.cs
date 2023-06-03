using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    Runtime Runtime,
    byte[] Logo) : IMinecraftProject
{
    public async Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null,
        CancellationToken cancellationToken = default!)
    {
        return await SpigotVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }    
}