using System.ComponentModel.Composition;
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
        string projectName,
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        return await FabricVersionFactory.GetVersion(projectName, options ?? new VersionOptions(), cancellationToken);
    }
    
}