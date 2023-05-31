﻿using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang;

[Export(typeof(IMinecraftProvider))]
public class MojangProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public MojangProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        MojangVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }

    public ProviderOptions ProviderOptions { get; }
    public string Name => "Mojang";
    public byte[] Logo => Properties.Resources.Mojang;
    public IEnumerable<IMinecraftProject> Projects => MojangProjectFactory.Projects;

    public async Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var versionOptions = options ?? new VersionOptions();
        var versions = new List<IMinecraftVersion>();

        foreach (var project in Projects)
        {
            versions.AddRange(await project.GetVersions(versionOptions, cancellationToken));
        }

        return versions;
    }     
}