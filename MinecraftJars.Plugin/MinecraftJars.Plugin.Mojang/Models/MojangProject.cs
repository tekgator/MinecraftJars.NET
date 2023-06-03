using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang.Models;

public record MojangProject(
    ProjectGroup ProjectGroup,
    string Name,
    string Description,
    string Url,
    ProjectRuntime ProjectRuntime,
    byte[] Logo) : IMinecraftProject
{
    public async Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null,
        CancellationToken cancellationToken = default!)
    {
        return await MojangVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }   
}