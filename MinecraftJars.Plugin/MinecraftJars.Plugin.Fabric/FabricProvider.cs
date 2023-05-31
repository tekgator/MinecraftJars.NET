using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Fabric;

[Export(typeof(IMinecraftProvider))]
public class FabricProvider : IMinecraftProvider
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