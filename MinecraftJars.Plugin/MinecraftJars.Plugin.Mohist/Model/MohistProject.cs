﻿using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistProject(
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
        return await MohistVersionFactory.GetVersion(Name, options ?? new VersionOptions(), cancellationToken);
    }   
}
