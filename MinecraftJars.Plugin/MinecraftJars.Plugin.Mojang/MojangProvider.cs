﻿using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang;

[Export(typeof(IProvider))]
public class MojangProvider : IProvider
{
    [ImportingConstructor]
    public MojangProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        MojangVersionFactory.HttpClientFactory = ProviderOptions.HttpClientFactory;
    }

    public ProviderOptions ProviderOptions { get; }
    public string Name => Provider.Mojang;
    public byte[] Logo => Properties.Resources.Mojang;
    public IEnumerable<IProject> Projects => MojangProjectFactory.Projects;

    public async Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? versionOptions = null, 
        CancellationToken cancellationToken = default!)
    {
        return await MojangVersionFactory.Get(versionOptions ?? new VersionOptions(), cancellationToken);
    }     
}