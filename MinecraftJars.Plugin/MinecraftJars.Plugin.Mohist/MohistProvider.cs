using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist;

[Export(typeof(IMinecraftProvider))]
public class MohistProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public MohistProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        MohistVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Mohist";
    public byte[] Logo => Properties.Resources.Mohist;
    public IEnumerable<IMinecraftProject> Projects => MohistProjectFactory.Projects;
   
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