using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang.Models;

public record MojangProject(
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
        return await MojangVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }   
}