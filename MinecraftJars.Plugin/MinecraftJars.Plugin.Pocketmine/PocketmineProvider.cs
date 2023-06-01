using System.ComponentModel.Composition;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Pocketmine;

[Export(typeof(IMinecraftProvider))]
public class PocketmineProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public PocketmineProvider(
        PluginHttpClientFactory httpClientFactory, 
        ProviderOptions? options)
    {
        PocketmineVersionFactory.HttpClientFactory = httpClientFactory;
        ProviderOptions = options ?? new ProviderOptions();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Pocketmine";
    public byte[] Logo => Properties.Resources.Pocketmine;
    public IEnumerable<IMinecraftProject> Projects => PocketmineProjectFactory.Projects;

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