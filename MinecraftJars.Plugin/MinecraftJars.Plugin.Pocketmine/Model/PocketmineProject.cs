using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Pocketmine.Model;

public record PocketmineProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IMinecraftProject
{
    public async Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null,
        CancellationToken cancellationToken = default!)
    {
        return await PocketmineVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }        
}