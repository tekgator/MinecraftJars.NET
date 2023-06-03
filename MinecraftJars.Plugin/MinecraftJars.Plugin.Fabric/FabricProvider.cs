using System.ComponentModel.Composition;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Fabric;

[Export(typeof(IMinecraftProvider))]
public class FabricProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public FabricProvider(PluginHttpClientFactory httpClientFactory)
    {
        FabricVersionFactory.HttpClientFactory = httpClientFactory;
    }

    public string Name => "Fabric";
    public byte[] Logo => Properties.Resources.Fabric;
    public IEnumerable<IMinecraftProject> Projects => FabricProjectFactory.Projects;
   
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