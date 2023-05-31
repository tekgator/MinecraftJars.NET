using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Pocketmine;

[Export(typeof(IProvider))]
public class PocketmineProvider : IProvider
{
    [ImportingConstructor]
    public PocketmineProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        PocketmineVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }
    
    public ProviderOptions ProviderOptions { get; }
    
    public string Name => "Pocketmine";
    public byte[] Logo => Properties.Resources.Pocketmine;
    public IEnumerable<IProject> Projects => PocketmineProjectFactory.Projects;

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