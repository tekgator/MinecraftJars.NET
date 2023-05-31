﻿using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Fabric;

[Export(typeof(IProvider))]
public class FabricProvider : IProvider
{
    [ImportingConstructor]
    public FabricProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        FabricVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Fabric";
    public byte[] Logo => Properties.Resources.Fabric;
    public IEnumerable<IProject> Projects => FabricProjectFactory.Projects;
   
    public async Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var versionOptions = options ?? new VersionOptions();
        var versions = new List<IVersion>();

        foreach (var project in Projects)
        {
            versions.AddRange(await project.GetVersions(versionOptions, cancellationToken));
        }

        return versions;
    }
}